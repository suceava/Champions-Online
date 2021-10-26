$(document).ready(function() {
	Builder.CO.Build.initializeBuild();
});

/* namespace */
Builder = function() {}
Builder.CO = function() {}


/* enum */
Builder.CO.Statistics = {
	InnateTalents : 0,
	Powers : 1,
	TravelPowers : 2,
	Advantages : 3,
	CharacteristicFocus : 4,
	Talents : 5
}

/* build button data */
Builder.CO.BuildButton = function(buttons)
{
	this.init(buttons);
}
Builder.CO.BuildButton.prototype = {
	Buttons : null,
	CurrentIndex : 0,
	
	init : function(buttons)
	{
		this.Buttons = buttons;
	}
}
Builder.CO.BuildButtonAdv = function() {
	this.PointsUsed = 0;
	this.MaxPoints = 5;
	this.Advs = new Array();
}

/* the static Build class */
Builder.CO.Build = {
	/* constants */
	EMPTY_POWER: { Name: 'EmptyPower', ImageName: 'images/Powerhouse/Powers/', Code: '0' },
	
	SUMMARY_LIST_HEADER: '<h3 class="summary-list-header bada">{0}</h3>',
	SUMMARY_SUBLIST_HEADER: '<h4 class="summary-sublist-header bada">{0}</h4>',
	SUMMARY_LIST_BODY: '{1}<ul>{0}</ul>',
	SUMMARY_LIST_ITEM: '<li>{0}</li>',

	BUILD_INNATE_LEVELS: [1],
	BUILD_TALENT_LEVELS: [6, 9, 12, 15, 18, 21],
	BUILD_CHARFOCUS_LEVELS: [5, 13],
	BUILD_TRAVEL_LEVELS: [5, 35],
	BUILD_POWERS_LEVELS: [1, 1, 5, 8, 11, 14, 17, 20, 23, 26, 29, 32, 35, 38],
	// advantage points come in 2s
	BUILD_ADVANTAGES_LEVELS: [7, 7, 10, 10, 16, 16, 19, 19, 22, 22, 24, 24, 25, 25, 27, 27, 28, 28, 30, 30, 31, 31, 33, 33, 34, 34, 36, 36, 37, 37, 39, 39, 40, 40],
	/* end constants */

	buildData: null,
	selectedTab: Builder.CO.Statistics.InnateTalents,
	selectedPowerset: 0,

	/* build selection */
	build_innateTalents: null,
	build_talents: null,
	build_charfocus: null,
	build_travelPowers: null,
	build_powers: null,
	build_advantages: null,

	initializeBuild: function() {
		$('#accordionTabs img.groupImg').click(function(event, ui) {
			Builder.CO.Build.selectTab($('#accordionTabs img.groupImg').index(this));
		});
		$('#accordionTabs .removeStat').click(function(event, ui) {
			Builder.CO.Build.unpurchaseStatistic();
		});

		if (Builder.CO.List) {
			Builder.CO.List.initializeList();
			Builder.CO.Tab.initializeTab();
		}

		// create build lists
		this.createBuildLists();

		// get powers data from server
		this.getData();

		this.copyHash();
	},
	getData: function() {
		var me = this;

		this.callService('BuildData', '', function(data) {
			me.buildData = data;

			// add powerset index to powers
			for (var i = 0, l = data.Powersets.length; i < l; i++) {
				var pset = data.Powersets[i];
				for (var j = 0, m = pset.Powers.length; j < m; j++) {
					var power = pset.Powers[j];
					power.PowersetIndex = i;

					// add power to advantage
					for (var k = 0, lk = power.Advantages.length; k < lk; k++) {
						power.Advantages[k].Power = power;
					}
				}
			}

			// restore
			Builder.CO.Build.restoreFromHash();

			if (Builder.CO.List) {
				// create the powerset tabs
				Builder.CO.Tab.loadTabs(data.Powersets);

				// select the first tab
				Builder.CO.Build.selectTab(Builder.CO.Statistics.InnateTalents);
				
				// refresh the counts
				Builder.CO.Build.refreshCount();
			}
			else if (window.location.toString().toLowerCase().indexOf('summary') > 0) {
				Builder.CO.Build.writeSummary();
			}
		});
	},
	createBuildLists: function() {
		this.build_innateTalents = new Builder.CO.BuildButton(this.createBuildList(this.BUILD_INNATE_LEVELS.length, $('#buildInnateTalent'), Builder.CO.Statistics.InnateTalents));
		this.build_talents = new Builder.CO.BuildButton(this.createBuildList(this.BUILD_TALENT_LEVELS.length, $('#buildTalents'), Builder.CO.Statistics.Talents));
		this.build_charfocus = new Builder.CO.BuildButton(this.createBuildList(this.BUILD_CHARFOCUS_LEVELS.length, $('#buildCharacteristicFocus'), Builder.CO.Statistics.CharacteristicFocus));
		this.build_travelPowers = new Builder.CO.BuildButton(this.createBuildList(this.BUILD_TRAVEL_LEVELS.length, $('#buildTravelPowers'), Builder.CO.Statistics.TravelPowers));
		this.build_powers = new Builder.CO.BuildButton(this.createBuildList(this.BUILD_POWERS_LEVELS.length, $('#buildPowers'), Builder.CO.Statistics.Powers));
		this.build_advantages = new Builder.CO.BuildButton(null);
	},
	createBuildList: function(max, parent, statType) {
		var list = new Array();
		for (var i = 0; i < max; i++) {
			list[i] = new Builder.CO.Button(this.EMPTY_POWER, parent, statType, undefined, undefined);
		}
		list[0].setSelected();
		return list;
	},
	refreshBuildList: function(buttonList) {
		if (!buttonList.Buttons) return; // advantages don't have buttons

		var curIndex = buttonList.CurrentIndex;
		for (var i = 0, l = buttonList.Buttons.length; i < l; i++) {
			var btn = buttonList.Buttons[i];
			if (i == curIndex)
				btn.setSelected();
			else
				btn.setNormal();
		}
	},
	selectTab: function(index) {
		// clear all selected
		$('#accordionTabs img.groupImg').each(function(i) {
			var img = $(this);
			Builder.CO.Build.changeImage($(this), '_Selected', true);
			img.next().next().toggle(false).next().toggle(false);
		});

		// get the current tab image
		var img = $('#accordionTabs img.groupImg:eq(' + index + ')');
		Builder.CO.Build.changeImage(img, '_Selected', false);
		img.next().next().toggle(true).next().toggle(true);

		Builder.CO.Build.selectedTab = index;
		Builder.CO.Tab.hideTab();

		var currentIndex;
		switch (index) {
			case Builder.CO.Statistics.InnateTalents:
				Builder.CO.List.load(this.buildData.InnateTalents, this.build_innateTalents, this.build_innateTalents.CurrentIndex < this.BUILD_INNATE_LEVELS.length, Builder.CO.Statistics.InnateTalents);
				currentIndex = this.build_innateTalents.CurrentIndex;
				break;
			case Builder.CO.Statistics.Powers:
				this.selectPowerset(this.selectedPowerset);
				currentIndex = this.build_powers.CurrentIndex;
				Builder.CO.Tab.showTab();
				break;
			case Builder.CO.Statistics.TravelPowers:
				Builder.CO.List.load(this.buildData.TravelPowers, this.build_travelPowers, this.build_travelPowers.CurrentIndex < this.BUILD_TRAVEL_LEVELS.length, Builder.CO.Statistics.TravelPowers);
				currentIndex = this.build_travelPowers.CurrentIndex;
				break;

			case Builder.CO.Statistics.Advantages:
				Builder.CO.List.loadAdvantages(this.build_powers, this.build_advantages, this.build_advantages.CurrentIndex < this.BUILD_ADVANTAGES_LEVELS.length);
				currentIndex = 0;
				Builder.CO.List.refreshAdvantageButtons();

				// hook up unpurchase
				$('.removeAdvantage').click(function(event, ui) {
					var grp = $(this).parent();
					if (!grp) return;
					var pwr = Builder.CO.Build.findPowerButton({ Name: grp.children(':first').text() });
					if (!pwr) return;

					Builder.CO.Build.unpurchaseStatistic(pwr);
				});
				break;

			case Builder.CO.Statistics.CharacteristicFocus:
				Builder.CO.List.load(this.buildData.Characteristics, this.build_charfocus, this.build_charfocus.CurrentIndex < this.BUILD_CHARFOCUS_LEVELS.length, Builder.CO.Statistics.CharacteristicFocus);
				currentIndex = this.build_charfocus.CurrentIndex;
				break;
			case Builder.CO.Statistics.Talents:
				Builder.CO.List.load(this.buildData.Talents, this.build_talents, this.build_talents.CurrentIndex < this.BUILD_TALENT_LEVELS.length, Builder.CO.Statistics.Talents);
				currentIndex = this.build_talents.CurrentIndex;
				break;
		}

		// hide minus button if no purchases have been made
		if (currentIndex == 0 && index != Builder.CO.Statistics.Advantages)
			img.next().next().toggle(false);
	},
	selectPowerset: function(index) {
		this.selectedPowerset = index;
		Builder.CO.List.load(this.buildData.Powersets[this.selectedPowerset].Powers, this.build_powers, this.build_powers.CurrentIndex < this.BUILD_POWERS_LEVELS.length, Builder.CO.Statistics.Powers);
		Builder.CO.List.refreshButtons(this.build_powers, this.getTotalPoints(this.selectedPowerset));
	},
	refreshMinus: function() {
		var img = $('#accordionTabs img.removeStat:eq(' + this.selectedTab + ')');
		var currentIndex;
		switch (this.selectedTab) {
			case Builder.CO.Statistics.InnateTalents:
				currentIndex = this.build_innateTalents.CurrentIndex;
				break;
			case Builder.CO.Statistics.Powers:
				currentIndex = this.build_powers.CurrentIndex;
				break;
			case Builder.CO.Statistics.TravelPowers:
				currentIndex = this.build_travelPowers.CurrentIndex;
				break;
			case Builder.CO.Statistics.CharacteristicFocus:
				currentIndex = this.build_charfocus.CurrentIndex;
				break;
			case Builder.CO.Statistics.Talents:
				currentIndex = this.build_talents.CurrentIndex;
				break;
		}
		img.toggle(currentIndex > 0);
	},
	refreshCount: function() {
		this.refreshTabCount(Builder.CO.Statistics.InnateTalents, this.build_innateTalents.CurrentIndex.toString() + '/' + this.BUILD_INNATE_LEVELS.length.toString());
		this.refreshTabCount(Builder.CO.Statistics.Powers, this.build_powers.CurrentIndex.toString() + '/' + this.BUILD_POWERS_LEVELS.length.toString());
		this.refreshTabCount(Builder.CO.Statistics.TravelPowers, this.build_travelPowers.CurrentIndex.toString() + '/' + this.BUILD_TRAVEL_LEVELS.length.toString());
		this.refreshTabCount(Builder.CO.Statistics.Advantages, this.build_advantages.CurrentIndex.toString() + '/' + this.BUILD_ADVANTAGES_LEVELS.length.toString());
		this.refreshTabCount(Builder.CO.Statistics.CharacteristicFocus, this.build_charfocus.CurrentIndex.toString() + '/' + this.BUILD_CHARFOCUS_LEVELS.length.toString());
		this.refreshTabCount(Builder.CO.Statistics.Talents, this.build_talents.CurrentIndex.toString() + '/' + this.BUILD_TALENT_LEVELS.length.toString());
	},
	refreshTabCount: function(tabIndex, count) {
		$('#accordionTabs div.accordionTabCounter:eq(' + tabIndex + ')').text(count);
	},
	purchaseStatistic: function() {
		// purchase a statistic

		var list = Builder.CO.List;
		if (!list._selectedButton) return;

		var data = list._selectedButton.Data;
		if (!data) return;

		switch (Builder.CO.Build.selectedTab) {
			case Builder.CO.Statistics.InnateTalents:
				this.processPurchase(this.BUILD_INNATE_LEVELS, this.build_innateTalents, data);
				break;

			case Builder.CO.Statistics.Powers:
				this.processPurchase(this.BUILD_POWERS_LEVELS, this.build_powers, data);
				// refresh the purchase list
				Builder.CO.List.refreshButtons(this.build_powers, this.getTotalPoints(this.selectedPowerset));
				break;

			case Builder.CO.Statistics.TravelPowers:
				this.processPurchase(this.BUILD_TRAVEL_LEVELS, this.build_travelPowers, data);
				break;

			case Builder.CO.Statistics.Advantages:
				this.processPurchase(this.BUILD_ADVANTAGES_LEVELS, this.build_advantages, data);
				Builder.CO.List.refreshAdvantageButtons();
				break;

			case Builder.CO.Statistics.CharacteristicFocus:
				this.processPurchase(this.BUILD_CHARFOCUS_LEVELS, this.build_charfocus, data);
				break;

			case Builder.CO.Statistics.Talents:
				this.processPurchase(this.BUILD_TALENT_LEVELS, this.build_talents, data);
				break;
		}
		this.refreshMinus();
		this.refreshCount();

		// refresh hash
		this.createHash();
	},
	processPurchase: function(levelsList, buttonsList, data) {
		if (buttonsList.CurrentIndex >= levelsList.length) return;

		if (data.Cost) {
			var pwr = Builder.CO.Build.findPowerButton(data.Power);
			if (!pwr) return;

			pwr.PurchasedAdvantages.Advs.push(data);
			pwr.PurchasedAdvantages.PointsUsed += data.Cost;
			buttonsList.CurrentIndex += data.Cost;

			var grp = Builder.CO.List.findGroupHeader(Builder.CO.List.findButton(data));
			if (grp) {
				grp.updateCounter(pwr.PurchasedAdvantages.PointsUsed, pwr.PurchasedAdvantages.MaxPoints);
			}
		}
		else {
			// set the button's data
			buttonsList.Buttons[buttonsList.CurrentIndex++].Data = data;
		}

		// mark the stat button as purchased
		Builder.CO.List._selectedButton.setChecked();
		// disable the purchase button
		Builder.CO.List.changePurchaseButtonEnabled(false);
		Builder.CO.List._canPurchase = (buttonsList.CurrentIndex < levelsList.length);
		// refresh the build list
		this.refreshBuildList(buttonsList);
	},
	unpurchaseStatistic: function(power) {
		// unpurchase statistic

		switch (Builder.CO.Build.selectedTab) {
			case Builder.CO.Statistics.InnateTalents:
				this.processUnpurchase(this.BUILD_INNATE_LEVELS, this.build_innateTalents);
				break;

			case Builder.CO.Statistics.Powers:
				this.processUnpurchase(this.BUILD_POWERS_LEVELS, this.build_powers);
				// refresh the purchase list
				Builder.CO.List.refreshButtons(this.build_powers, this.getTotalPoints(this.selectedPowerset));
				break;

			case Builder.CO.Statistics.TravelPowers:
				this.processUnpurchase(this.BUILD_TRAVEL_LEVELS, this.build_travelPowers);
				break;

			case Builder.CO.Statistics.Advantages:
				// refresh list
				this.processUnpurchase(this.BUILD_ADVANTAGES_LEVELS, this.build_advantages, power);
				//Builder.CO.List.refreshAdvantageButtons();
				break;

			case Builder.CO.Statistics.CharacteristicFocus:
				this.processUnpurchase(this.BUILD_CHARFOCUS_LEVELS, this.build_charfocus);
				break;

			case Builder.CO.Statistics.Talents:
				this.processUnpurchase(this.BUILD_TALENT_LEVELS, this.build_talents);
				break;
		}

		// refresh list
		this.selectTab(Builder.CO.Build.selectedTab)
		this.refreshMinus();
		this.refreshCount();

		// refresh hash
		this.createHash();
	},
	processUnpurchase: function(levelsList, buttonsList, power) {
		if (buttonsList.CurrentIndex == 0) return;

		if (power) {
			if (power.PurchasedAdvantages.Advs.length == 0) return;

			var data = power.PurchasedAdvantages.Advs.pop();
			power.PurchasedAdvantages.PointsUsed -= data.Cost;
			this.build_advantages.CurrentIndex -= data.Cost;

			var grp = Builder.CO.List.findGroupHeader(Builder.CO.List.findButton(data));
			if (grp) {
				grp.updateCounter(power.PurchasedAdvantages.PointsUsed, power.PurchasedAdvantages.MaxPoints);
			}
		}
		else {
			// set the button's data
			var button = buttonsList.Buttons[--buttonsList.CurrentIndex];
			button.Data = this.EMPTY_POWER;
			if (button.PurchasedAdvantages !== undefined)
				delete button.PurchasedAdvantages;
		}

		Builder.CO.List._canPurchase = (buttonsList.CurrentIndex < levelsList.length);

		// refresh the build list
		this.refreshBuildList(buttonsList);
	},
	copyHash: function() {
		var hash = window.location.hash;

		$('#homeLink').attr('href', '/' + hash);
		$('#aboutLink').attr('href', 'Home/About/' + hash);
		$('#summaryLink').attr('href', 'Home/Summary/' + hash);
	},
	createHash: function() {
		var hash = '';
		// innate talents
		hash = this.createHashFromList(this.build_innateTalents);
		// char focus
		hash += this.createHashFromList(this.build_charfocus);
		// talents
		hash += this.createHashFromList(this.build_talents);
		// travel powers
		hash += this.createHashFromList(this.build_travelPowers);
		// powers
		for (var i = 0, l = this.build_powers.Buttons.length; i < l; i++) {
			var power = this.build_powers.Buttons[i].Data;

			// add the powerset
			hash += (typeof power.PowersetIndex != 'undefined' ? this.buildData.Powersets[power.PowersetIndex].Code : '0');
			// add the power
			hash += power.Code;
			// add the advantages
			hash += this.createHashForAdvantages(this.build_powers.Buttons[i]);
		}

		window.location.hash = hash;

		// copy to other links
		this.copyHash();
	},
	createHashFromList: function(buttonsList) {
		var hash = '';
		for (var i = 0, l = buttonsList.Buttons.length; i < l; i++) {
			hash += buttonsList.Buttons[i].Data.Code;
		}
		return hash;
	},
	createHashForAdvantages: function(powerButton) {
		if (!powerButton.PurchasedAdvantages || !powerButton.Data) return '00';

		var h = 0;
		for (var i = 0, li = powerButton.Data.Advantages.length; i < li; i++) {
			for (var j = 0, lj = powerButton.PurchasedAdvantages.Advs.length; j < lj; j++) {
				if (powerButton.Data.Advantages[i] == powerButton.PurchasedAdvantages.Advs[j]) {
					h += (1 << i);
					break;
				}
			}
			//			console.debug('adv: ' + powerButton.Data.Advantages[i].Name + '  ' + ((h >> i) ? 'yes':'no'));
		}

		return (h < 10 ? '0' : '') + h.toString();
	},
	restoreFromHash: function() {
		var hash = window.location.hash;
		if (hash == '') return;

		// innate talents
		var index = 1;
		this.restoreListFromHash(hash.substring(index, index + this.BUILD_INNATE_LEVELS.length), this.buildData.InnateTalents, this.build_innateTalents);
		// char focus
		index += this.BUILD_INNATE_LEVELS.length;
		this.restoreListFromHash(hash.substring(index, index + this.BUILD_CHARFOCUS_LEVELS.length), this.buildData.Characteristics, this.build_charfocus);
		// talents
		index += this.BUILD_CHARFOCUS_LEVELS.length;
		this.restoreListFromHash(hash.substring(index, index + this.BUILD_TALENT_LEVELS.length), this.buildData.Talents, this.build_talents);
		// travel powers
		index += this.BUILD_TALENT_LEVELS.length;
		this.restoreListFromHash(hash.substring(index, index + this.BUILD_TRAVEL_LEVELS.length), this.buildData.TravelPowers, this.build_travelPowers);
		// powers
		index += this.BUILD_TRAVEL_LEVELS.length;
		var ph = hash.substring(index, index + this.BUILD_POWERS_LEVELS.length * 4);
		for (var i = 0, l = ph.length; i < l; i += 4) {
			for (var j = 0, lj = this.buildData.Powersets.length; j < lj; j++) {
				var pset = this.buildData.Powersets[j];
				if (pset.Code == ph[i]) {
					for (var k = 0, lk = pset.Powers.length; k < lk; k++) {
						if (pset.Powers[k].Code == ph[i + 1]) {
							this.build_powers.Buttons[this.build_powers.CurrentIndex++].Data = pset.Powers[k];

							// advantages
							this.restoreAdvantagesFromHash(ph[i + 2] + ph[i + 3], this.build_powers.Buttons[this.build_powers.CurrentIndex - 1], this.build_advantages);
							break;
						}
					}
					break;
				}
			}
		}
		this.refreshBuildList(this.build_powers);
	},
	restoreListFromHash: function(hash, data, buttonsList) {
		for (var i = 0, l = hash.length; i < l; i++) {
			for (var j = 0, ll = data.length; j < ll; j++) {
				if (data[j].Code == hash[i]) {
					buttonsList.Buttons[buttonsList.CurrentIndex++].Data = data[j];
					break;
				}
			}
		}
		this.refreshBuildList(buttonsList);
	},
	restoreAdvantagesFromHash: function(hash, powerButton, advantages) {
		if (powerButton.PurchasedAdvantages === undefined)
			powerButton.PurchasedAdvantages = new Builder.CO.BuildButtonAdv();

		// get actual number
		var h = 0;
		try {
			h = Number(hash);
		}
		catch (ex) {
			return;
		}

		// loop through available advantages and see which were purchased
		for (var i = 0, li = powerButton.Data.Advantages.length; i < li; i++) {
			if ((1 << i) & h) {
				var adv = powerButton.Data.Advantages[i];
				powerButton.PurchasedAdvantages.Advs.push(adv);
				powerButton.PurchasedAdvantages.PointsUsed += adv.Cost;
				advantages.CurrentIndex += adv.Cost;
			}
			//console.debug('adv: ' + powerButton.Data.Advantages[i].Name + '  ' + (((1 << i) & h) ? 'yes':'no'));
		}
	},
	getTotalPoints: function(powerset) {
		var total = { intotal: 0, outtotal: 0, group1total: 0, group2total: 0 };
		for (var i = 0, l = this.build_powers.CurrentIndex; i < l; i++) {
			var btn = this.build_powers.Buttons[i];
			// if the purchased power belongs to this powerset
			if (btn.Data.PowersetIndex == powerset ||
			// or the purchased power is in the same group as the powerset
				this.isSameGroupIndex(btn.Data.PowersetIndex, powerset, btn.Data.FloatingGroupPower)) {
				total.intotal++;
			}
			else {
				// count groups
				if (this.isSameGroup(this.buildData.Powersets[btn.Data.PowersetIndex].PowersetGroup, this.buildData.Powersets[powerset].PowersetGroup, 0)) {
					total.group1total++;
				}
				if (this.isSameGroup(this.buildData.Powersets[btn.Data.PowersetIndex].PowersetGroup, this.buildData.Powersets[powerset].PowersetGroup, 1)) {
					total.group2total++;
				}
			}

			total.outtotal++;
		}
		// adjust for energy-builder
		if (total.outtotal > 0)
			total.outtotal--;
		return total;
	},
	getPowersetName: function(powerset) {
		if (!this.buildData || powerset < 0 || powerset >= this.buildData.Powersets.length)
			return '';

		return this.buildData.Powersets[powerset].Name;
	},
	getPowersetGroupName: function(powerset, index) {
		if (!this.buildData || powerset < 0 || powerset >= this.buildData.Powersets.length)
			return '';

		var groups = this.buildData.Powersets[powerset].PowersetGroup.split(',');
		return groups[index < groups.length ? index : 0];
	},
	isSameGroup: function(group1, group2, index) {
		var grps1 = group1.split(',');
		var grps2 = group2.split(',');
		if (grps1.length < index || grps2.length < index) return false;

		return grps1[index] == grps2[index];
	},
	isSameGroupIndex: function(index1, index2, floatingGroupPower) {
		if (floatingGroupPower == 0) return false;

		var group1 = this.buildData.Powersets[index1].PowersetGroup;
		var group2 = this.buildData.Powersets[index2].PowersetGroup;
		//return this.isSameGroup(group1, group2, floatingGroupPower);

		var index = floatingGroupPower - 1;
		var grps1 = group1.split(',');
		var grps2 = group2.split(',');
		if (grps1.length < index || grps2.length < index) return false;

		return grps1[index] == grps2[index];
	},
	findPowerButton: function(power) {
		// loop through purchased powers
		for (var i = 0, li = this.build_powers.CurrentIndex; i < li; i++) {
			var pwr = this.build_powers.Buttons[i];
			if (pwr.Data.Name == power.Name)
				return pwr;
		}
		return null
	},

	changeImage: function(img, postfix, remove) {
		var src = img.attr('src');
		if (!src) return;

		if (remove) // && !img.hasClass('tabselected'))
		{
			src = src.replace(postfix + '.png', '.png');
		}
		else if (!remove && src.indexOf(postfix) < 0) {
			src = src.replace('.png', postfix + '.png');
		}
		img.attr('src', src);
	},
	writeSummary: function() {
		var list = '';
		// innate talents
		list += this.writeSummaryFromList('Innate Talent', this.build_innateTalents);
		// char focus
		list += this.writeSummaryFromList('Characteristic Focus', this.build_charfocus);
		// talents
		list += this.writeSummaryFromList('Talents', this.build_talents);
		// travel powers
		list += this.writeSummaryFromList('Travel Powers', this.build_travelPowers);
		// powers		
		var listPowers = '', listP = '';
		for (var i = 0, l = this.build_powers.Buttons.length; i < l; i++) {
			var power = this.build_powers.Buttons[i].Data;
			if (power.Name == 'EmptyPower') continue;

			// add the powerset
			listP = (typeof power.PowersetIndex != 'undefined' ? this.buildData.Powersets[power.PowersetIndex].Name + ' - ' : '');
			// add the power
			listP += power.Name;
			// add the advantages
			listP += this.writeSummaryForAdvantages(this.build_powers.Buttons[i]);

			listPowers += this.SUMMARY_LIST_ITEM.replace('{0}', listP);
		}
		if (listPowers != '')
			list += this.SUMMARY_LIST_BODY.replace('{0}', listPowers).replace('{1}', this.SUMMARY_LIST_HEADER.replace('{0}', 'Powers'));
		
		$(list).appendTo('#summary');
	},
	writeSummaryFromList: function(name, buttonsList) {
		var list = '';
		for (var i = 0, l = buttonsList.Buttons.length; i < l; i++) {
			if (buttonsList.Buttons[i].Data.Name != 'EmptyPower')
				list += this.SUMMARY_LIST_ITEM.replace('{0}', buttonsList.Buttons[i].Data.Name);
		}
		if (list == '') return '';
		
		return this.SUMMARY_LIST_BODY.replace('{0}', list).replace('{1}', this.SUMMARY_LIST_HEADER.replace('{0}', name));
	},
	writeSummaryForAdvantages: function(powerButton) {
		if (!powerButton.PurchasedAdvantages || !powerButton.Data) return '';

		var h = 0, list = '';
		for (var i = 0, li = powerButton.Data.Advantages.length; i < li; i++) {
			for (var j = 0, lj = powerButton.PurchasedAdvantages.Advs.length; j < lj; j++) {
				if (powerButton.Data.Advantages[i] == powerButton.PurchasedAdvantages.Advs[j]) {
					list += this.SUMMARY_LIST_ITEM.replace('{0}', powerButton.Data.Advantages[i].Name);
					break;
				}
			}
		}
		if (list == '') return '';

		return this.SUMMARY_LIST_BODY.replace('{0}', list).replace('{1}', this.SUMMARY_SUBLIST_HEADER.replace('{0}', 'Advantages'));
	},

	callService: function(method, data, callback, async) {
		if (typeof async === 'undefined')
			async = true;
		url = '/Home/';
		$.ajax({
			type: 'POST',
			url: url + method,
			async: async,
			data: data,
			dataType: 'json',
			contentType: 'application/json; charset=utf-8',
			success: function(data, textStatus) {
				callback(data);
			},
			error: function(xhr, textStatus, errorThrown) {
				var rt;
				if (xhr && xhr.responseText) {
					try {
						rt = eval('(' + xhr.responseText + ')');
					}
					catch (e) { alert(e); }
				}
				if (rt && rt.Message)
					alert(rt.Message);
				else
					alert('There was an error connecting to the server.  Try refreshing the page.');
			}
		});
	}
}