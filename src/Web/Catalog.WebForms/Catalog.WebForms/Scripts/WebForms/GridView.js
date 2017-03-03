//CdnPath=http://ajax.aspnetcdn.com/ajax/4.5.1/1/GridView.js
function GridView() {
    this.pageIndex = null;
    this.sortExpression = null;
    this.sortDirection = null;
    this.dataKeys = null;
    this.createPropertyString = GridView_createPropertyString;
    this.setStateField = GridView_setStateValue;
    this.getHiddenFieldContents = GridView_getHiddenFieldContents;
    this.stateField = null;
    this.panelElement = null;
    this.callback = null;
}
function GridView_createPropertyString() {
    return createPropertyStringFromValues_GridView(this.pageIndex, this.sortDirection, this.sortExpression, this.dataKeys);
}
function GridView_setStateValue() {
    this.stateField.value = this.createPropertyString();
}
function GridView_OnCallback (result, context) {
    var value = new String(result);
    var valsArray = value.split("|");
    var innerHtml = valsArray[4];
    for (var i = 5; i < valsArray.length; i++) {
        innerHtml += "|" + valsArray[i];
    }
    context.panelElement.innerHTML = innerHtml;
    context.stateField.value = createPropertyStringFromValues_GridView(valsArray[0], valsArray[1], valsArray[2], valsArray[3]);
}
function GridView_getHiddenFieldContents(arg) {
    return arg + "|" + this.stateField.value;
}
function createPropertyStringFromValues_GridView(pageIndex, sortDirection, sortExpression, dataKeys) {
    var value = new Array(pageIndex, sortDirection, sortExpression, dataKeys);
    return value.join("|");
}
