/* represents a power or attribute button in the list */
Builder.CO.Button = function(data, parent, statType, onclick, ondoubleclick)
{
	this.init(data, parent, statType, onclick, ondoubleclick);
}
Builder.CO.Button.prototype =
{
	Data: null,
	Parent: null,
	Node: null,
	StatType: 0,

	_disabled: false,
	_selected: false,
	_checked: false,
	_canRemove: false,

	init: function(data, parent, statType, onclick, ondoubleclick) {
		this.Data = data;
		this.StatType = statType;
		this.Parent = parent;
		this.Node = $('<div class="button"></div>').appendTo(parent);

		if (statType == Builder.CO.Statistics.InnateTalents ||
			statType == Builder.CO.Statistics.Talents) {
			this.Node.addClass('text_button');
			$('<div class="image" />').appendTo(this.Node);
			var txt = $('<div class="caption bada" />').appendTo(this.Node);
		}
		else if (statType == Builder.CO.Statistics.Advantages) {
			this.Node.addClass('long_text_button');
			var txt = $('<div class="caption bada" />').appendTo(this.Node);
			$('<div class="points bada" />').appendTo(this.Node);
		}
		else {
			this.Node.addClass('image_button');
		}

		this.setNormal();

		if (typeof onclick !== 'undefined') {
			this.Node.bind('click', this, onclick);
		}
		if (typeof ondoubleclick !== 'undefined') {
			this.Node.bind('dblclick', this, ondoubleclick);
		}
	},
	setNormal: function() {
		this._disabled = false;
		this._selected = false;
		this._checked = false;
		this.setBackgroundImage('');
	},
	setSelected: function(select) {
		if (typeof select === 'undefined') select = true;

		this._selected = select;
		if (this._selected)
			this.setBackgroundImage('selected');
		else if (this._disabled)
			this.setDisabled();
		else if (this._checked)
			this.setChecked();
		else
			this.setNormal();
	},
	setChecked: function() {
		if (this._disabled) return;

		this._checked = true;
		this.setBackgroundImage('purchased');
	},
	setDisabled: function() {
		this._disabled = true;
		this._checked = false;
		this.setBackgroundImage('disabled');
		this.Node.css('cursor', 'default');
	},
	setBackgroundImage: function(suffix) {
		var path;
		if (this.StatType == Builder.CO.Statistics.InnateTalents ||
			this.StatType == Builder.CO.Statistics.Talents ||
			this.StatType == Builder.CO.Statistics.Advantages) {
			// text
			$('.caption', this.Node).text((this.Data.Name != 'EmptyPower') ? this.Data.Name : '');
			// points
			if (this.Data.Cost)
				$('.points', this.Node).text(this.Data.Cost);
			this.Node.removeClass('selected').removeClass('disabled').removeClass('purchased').addClass(suffix);
		}
		else {
			if (suffix.length > 0)
				suffix = '_' + suffix;
			path = '/' + this.Data.ImageName + this.Data.Name + suffix + '.png';
			this.Node.css('backgroundImage', 'url("' + path + '")');
			this.Node.css('cursor', 'pointer');
		}
	},
	setToolTip: function() {
	}
}