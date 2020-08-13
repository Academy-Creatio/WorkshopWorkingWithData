define("ContactPageV2", [], function() {
	return {
		entitySchemaName: "Contact",
		attributes: {
			"MyCalculatedDuration": {
				"dataValueType": Terrasoft.DataValueType.INTEGER,
				"value": -5
			}
		},
		modules: /**SCHEMA_MODULES*/{}/**SCHEMA_MODULES*/,
		details: /**SCHEMA_DETAILS*/{}/**SCHEMA_DETAILS*/,
		businessRules: /**SCHEMA_BUSINESS_RULES*/{}/**SCHEMA_BUSINESS_RULES*/,
		methods: {
			onEntityInitialized: function() {
				this.callParent(arguments);
				this.calcContactActivitiesDuration();
			},
			calcContactActivitiesDuration: function() {
				var esq = this.Ext.create("Terrasoft.EntitySchemaQuery", {
					rootSchemaName: "Activity"
				});
				esq.addAggregationSchemaColumn("DurationInMinutes",
						this.Terrasoft.AggregationType.SUM, "DurationSum");
				var contactId = this.get("Id");
				var contactFilter = esq.createColumnFilterWithParameter(Terrasoft.ComparisonType.EQUAL,
						"Owner", contactId);
				esq.filters.addItem(contactFilter);
				esq.getEntityCollection(this.getCalcESQResult, this);
			},
			getCalcESQResult: function(response) {
				if (!response.success) {
					throw new Terrasoft.UnknownException();
				}
				var queryResult = response.collection.getByIndex(0);
				var duration = queryResult.get("DurationSum");
				this.set("MyCalculatedDuration", duration);
			}
		},
		dataModels: /**SCHEMA_DATA_MODELS*/{}/**SCHEMA_DATA_MODELS*/,
		diff: /**SCHEMA_DIFF*/[
			{
				"operation": "insert",
				"parentName": "ContactGeneralInfoBlock",
				"propertyName": "items",
				"name": "MyCalculatedDurationControl",
				"values": {
					"caption": "Calculated Duration",
					"layout": {
						"column": 12,
						"row": 3,
						"colSpan": 12,
						"rowSpan": 1
					},
					"bindTo": "MyCalculatedDuration",
					"enabled": false
				}
			}
		]/**SCHEMA_DIFF*/
	};
});
