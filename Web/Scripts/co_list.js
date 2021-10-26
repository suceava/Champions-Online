/* the list of available powers or attributes */
Builder.CO.List = {
	_list: null,
	_description: null,
	_purchaseButton: null,
	_canPurchase: true,
	_items: null,
	_groups: null,

	_powersetTab: null,
	_selectedTab: null,
	_selectedButton: null,
	_removableButton: null,

	initializeList: function() {
		this._list = $('#statisticsList');
		this._description = $('#description');

		this._purchaseButton = $('#purchaseButton').click(function() {
			var btn = $(this);
			if (btn.hasClass('disabled')) return false;

			// do purchase
			Builder.CO.Build.purchaseStatistic();
		}); ;
	},
	clear: function() {
		if (!this._items) return;

		this._list.empty();
		this._items = null;
		this._groups = null;
	},
	load: function(data, buildData, canPurchase, statType) {
		// data = file data
		// buildData = purchased build data
		this.clear();
		this.showDescription(null);
		this.changePurchaseButtonEnabled(false);
		this._canPurchase = canPurchase;

		if (!data) return;

		this._items = new Array();
		var oldGroup = '';
		for (var i = 0, l = data.length; i < l; i++) {
			if (typeof data[i].PowerType !== 'undefined' && data[i].PowerType) {
				// set up group before we create button
				if (data[i].PowerType != oldGroup) {
					// create group header
					oldGroup = data[i].PowerType;
					var grp = new Builder.CO.GroupHeader(oldGroup, this._list);
				}
			}

			var btn = new Builder.CO.Button(data[i], this._list, statType, Builder.CO.List.onButtonClick, Builder.CO.List.onButtonDoubleClick);
			// check for purchased stats
			for (var j = 0, ll = buildData.CurrentIndex; j < ll; j++) {
				var bld = buildData.Buttons[j];
				if (bld.Data.Name == data[i].Name) {
					btn.setChecked();
					break;
				}
			}

			this._items[i] = btn;
		}
	},
	loadAdvantages: function(powers, advantages, canPurchase) {
		// powers = purchased powers
		// advantages = purchased advantages
		this.clear();
		this.showDescription(null);
		this.changePurchaseButtonEnabled(false);
		this._canPurchase = canPurchase;

		if (!powers) return;

		this._items = new Array();
		this._groups = new Array();

		// loop through purchased powers
		for (var i = 0, li = powers.CurrentIndex; i < li; i++) {
			var powerBtn = powers.Buttons[i];
			var power = powerBtn.Data;
			// create a group header
			var grp = new Builder.CO.GroupHeader(power.Name, this._list, true);
			grp.updateCounter(0, 5);
			this._groups.push(grp);

			// loop through advantages
			for (var j = 0, lj = power.Advantages.length; j < lj; j++) {
				var adv = power.Advantages[j];
				var btn = new Builder.CO.Button(adv, this._list, Builder.CO.Statistics.Advantages, Builder.CO.List.onButtonClick, Builder.CO.List.onButtonDoubleClick);

				if (powerBtn.PurchasedAdvantages) {
					// check for purchased advantages
					for (var k = 0, lk = powerBtn.PurchasedAdvantages.Advs.length; k < lk; k++) {
						if (powerBtn.PurchasedAdvantages.Advs[k] == adv) {
							btn.setChecked();
							break;
						}
					}
					// update counter
					grp.updateCounter(powerBtn.PurchasedAdvantages.PointsUsed, powerBtn.PurchasedAdvantages.MaxPoints);
				}

				this._items.push(btn);
			}
		}
	},
	refreshButtons: function(buildData, totalPoints) {
		if (!this._items) return;

		for (var i = 0, l = this._items.length; i < l; i++) {
			var btn = this._items[i];

			// look for purchased
			var checked = false;
			for (var j = 0, m = buildData.Buttons.length; j < m; j++) {
				var d = buildData.Buttons[j].Data;
				// if the data on the button matches (e.g. same exact power)
				if (btn.Data == d ||
				// or the powers are in the same group and the powers have the same name (e.g. shared power)
					(Builder.CO.Build.isSameGroupIndex(btn.Data.PowersetIndex, Builder.CO.Build.selectedPowerset, btn.Data.FloatingGroupPower) &&
					btn.Data.Name == d.Name)) {
					btn.setChecked();
					checked = true;
					break;
				}
			}
			if (checked) continue;

			// look for energy builder
			var isEB = btn.Data.PowerType == 'Energy Builder';

			// see if group points should count for intotal
			var grp = 0;
			if (btn.Data.FloatingGroupPower == 1) {
				var grp1 = Builder.CO.Build.getPowersetGroupName(btn.Data.PowersetIndex, 0);
				if (grp1.length > 0) {
					grp += totalPoints.group1total;
				}
			}
			if (btn.Data.FloatingGroupPower == 2) {
				var grp2 = Builder.CO.Build.getPowersetGroupName(btn.Data.PowersetIndex, 1);
				if (grp2.length > 0) {
					grp += totalPoints.group2total;
				}
			}

			// if no power was purchased and this is not an energy builder power
			if ((buildData.CurrentIndex == 0 && !isEB) ||
			// or at least one power was purchased and this is an energy builder power
				(buildData.CurrentIndex > 0 && isEB) ||
			// or the intotal does not meet the requirements
				(btn.Data.InsetRequirement > totalPoints.intotal + grp &&
			// and the outtotal does not meet the requirements
					(btn.Data.OutsetRequirement > totalPoints.outtotal ||
			// or the in is more than the out requirement // this case is for new powers where inreq=10 and outreq=0
					btn.Data.InsetRequirement > btn.Data.OutsetRequirement)))
				btn.setDisabled();
			else
				btn.setNormal();
		}
	},
	refreshAdvantageButtons: function() {
		if (!this._items) return;

		for (var i = 0, li = this._items.length; i < li; i++) {
			var btn = this._items[i];

			if (btn.Data.RequiredAdvantage) {
				//console.dir(this._items[i-1]);
				if (!this._items[i - 1]._checked)
					btn.setDisabled();
				else if (!btn._checked)
					btn.setNormal();
			}
		}
	},
	onButtonClick: function(event) {
		var list = Builder.CO.List;
		if (list._selectedButton) {
			// unselect old button
			list._selectedButton.setSelected(false);
		}
		// select the new button
		list._selectedButton = event.data;
		list._selectedButton.setSelected();

		// can buy?
		var enablePurchase = list._canPurchase && !list._selectedButton._disabled && !list._selectedButton._checked;
		if (list._selectedButton.StatType == Builder.CO.Statistics.Advantages) {
			var adv = list._selectedButton.Data;
			var pwr = Builder.CO.Build.findPowerButton(adv.Power);
			if (pwr) {
				if (pwr.PurchasedAdvantages === undefined) {
					pwr.PurchasedAdvantages = new Builder.CO.BuildButtonAdv();
				}
				else if (pwr.PurchasedAdvantages.PointsUsed + adv.Cost > pwr.PurchasedAdvantages.MaxPoints) {
					enablePurchase = false;
				}
				else {
					// check for pre-req advantage
				}
			}
			else
				enablePurchase = false;
		}

		// show the description
		Builder.CO.List.showDescription(list._selectedButton.Data, enablePurchase);

		// enable the purchase button if necessary
		list.changePurchaseButtonEnabled(enablePurchase);
	},
	onButtonDoubleClick: function(event) {
		var list = Builder.CO.List;
		if (list._purchaseButton.hasClass('disabled')) return false;

		// do purchase
		Builder.CO.Build.purchaseStatistic();
		
		event.preventDefault();
	},
	changePurchaseButtonEnabled: function(enabled) {
		this._purchaseButton.toggleClass('disabled', !enabled);
		this._purchaseButton.css('cursor', enabled ? 'pointer' : 'default');
		Builder.CO.Build.changeImage(this._purchaseButton, '_Disabled', enabled);
	},
	showDescription: function(data, canPurchase) {
		var desc = '';
		if (data) {
			desc = '<h1>' + data.Name + '</h1>';
			if (data.InsetRequirement > 0) {
				desc += '<p id="desc_req" style="' + (canPurchase ? '' : 'color:red;') + '">Requires ' + data.InsetRequirement.toString() + ' powers from ';
				switch (data.FloatingGroupPower) {
					case 2: 	// sub-group power
						desc += Builder.CO.Build.getPowersetGroupName(data.PowersetIndex, 1) + ' Archtype';
						break;
					case 1: 	// group power
						desc += Builder.CO.Build.getPowersetGroupName(data.PowersetIndex, 0) + ' Archtype';
						break;
					default:
						desc += Builder.CO.Build.getPowersetName(data.PowersetIndex);
						break;
				}
			}
			if (data.OutsetRequirement > 0) {
				desc += ' or ' + data.OutsetRequirement.toString() + ' non-energy-building powers from any framework</p>';
			}
			desc += data.Description;

			if (desc.length == 0) desc = '<p>MISSING</p>';

			// see if we need to add the attributes
			var attrs = Builder.CO.Build.buildData.Stats;
			if (attrs && attrs.length > 0) {
				for (var i = 0, l = attrs.length; i < l; i++) {
					if (typeof data[attrs[i].Name] !== 'undefined') {
						var val = 0;
						try {
							val = Number(data[attrs[i].Name]);
						}
						catch (e) { }
						if (val > 0)
							desc += '<div style="margin-left:12px"><h1><span style="font-size:1.2em">+' + val + '</span> ' + attrs[i].Name + '</h1>' + attrs[i].Description + '</div>';
					}
				}
			}
		}

		this._description.html(desc);
	},
	findGroupHeader: function(button) {
		if (!this._groups) return null;

		var lastGrp;
		for (var i = 0, li = this._groups.length; i < li; i++) {
			var grp = this._groups[i];
			if (grp.Node.index() > button.Node.index())
				return lastGrp;
			lastGrp = grp;
		}
		return lastGrp;
	},
	findButton: function(power) {
		// loop through buttons
		for (var i = 0, li = this._items.length; i < li; i++) {
			var btn = this._items[i];
			if (btn.Data == power)
				return btn;
		}
		return null
	}
}


/* group headers for powers */
Builder.CO.GroupHeader = function(name, parent, showCounter)
{
	this.init(name, parent, showCounter);
}
Builder.CO.GroupHeader.prototype =
{
	Name: null,
	Parent: null,
	Node: null,
	Counter: null,

	init: function(name, parent, showCounter) {
		this.Name = name;
		this.Parent = parent;
		this.Node = $('<div class="groupHeader bada"><span>' + name + '</span></div>').appendTo(parent);
		if (showCounter) {
			$('<img src="/images/Powerhouse/minus16.png" class="removeAdvantage" title="Remove advantage" />').appendTo(this.Node);
			this.Counter = $('<div class="groupCounter"></div>').appendTo(this.Node);
		}
	},

	updateCounter: function(current, max) {
		this.Counter.text(current.toString() + '/' + max.toString());
	}
}

