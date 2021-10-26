/* powerset tabs */
Builder.CO.Tab = {
	_node: null,
	_tab: null,
	_data: null,
	_tabButtons: null,

	initializeTab: function()
	{
		this._node = $('#powersetTabs');
		this._tab = $('#powersetTabsList');
		this._tab.parent().css('padding', '1px');
	},
	loadTabs: function(data)
	{
		this._data = data;
		
		// clear tabs
		this._tabButtons = null;
		this._tab.empty();
		
		if (!data) return;
		
		this._tabButtons = new Array();
		for (var i=0,l=data.length; i<l; i++)
		{
			var d = data[i];
			var li = $('<li id="' + d.Name + '" title="' + d.Name + '" class="tabButton"></li>').appendTo(this._tab).click(this.onTabSelected);
			this.setBackgroundImage(li, d, '');
			
			this._tabButtons[i] = li;
		}
		// select the first one
		this.selectTab(0);
	},
	showTab: function()
	{
		this._node.show();
	},
	hideTab: function()
	{
		this._node.hide();
	},
	setBackgroundImage : function(li, data, suffix)
	{
		var path = '/' + data.ImageName + data.Name + suffix + '.png';
		li.css('backgroundImage', 'url("' + path + '")');
		li.css('cursor', 'pointer');
	},
	onTabSelected: function(event)
	{
		Builder.CO.Tab.selectTab($('.tabButton', Builder.CO.Tab._tab).index($(event.target)));
	},
	selectTab: function(index)
	{
		// unselect all
		for (var i=0, l=this._tabButtons.length; i<l; i++)
		{
			if (i == index)
				this.setBackgroundImage(this._tabButtons[i], this._data[i], '_Selected');
			else
				this.setBackgroundImage(this._tabButtons[i], this._data[i], '');
		}
		Builder.CO.Build.selectPowerset(index);
	}
}