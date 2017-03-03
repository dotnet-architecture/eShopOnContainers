//CdnPath=http://ajax.aspnetcdn.com/ajax/4.5.1/1/TreeView.js
function TreeView_HoverNode(data, node) {
    if (!data) {
        return;
    }
    node.hoverClass = data.hoverClass;
    WebForm_AppendToClassName(node, data.hoverClass);
    if (__nonMSDOMBrowser) {
        node = node.childNodes[node.childNodes.length - 1];
    }
    else {
        node = node.children[node.children.length - 1];
    }
    node.hoverHyperLinkClass = data.hoverHyperLinkClass;
    WebForm_AppendToClassName(node, data.hoverHyperLinkClass);
}
function TreeView_GetNodeText(node) {
    var trNode = WebForm_GetParentByTagName(node, "TR");
    var outerNodes;
    if (trNode.childNodes[trNode.childNodes.length - 1].getElementsByTagName) {
        outerNodes = trNode.childNodes[trNode.childNodes.length - 1].getElementsByTagName("A");
        if (!outerNodes || outerNodes.length == 0) {
            outerNodes = trNode.childNodes[trNode.childNodes.length - 1].getElementsByTagName("SPAN");
        }
    }
    var textNode = (outerNodes && outerNodes.length > 0) ?
        outerNodes[0].childNodes[0] :
        trNode.childNodes[trNode.childNodes.length - 1].childNodes[0];
    return (textNode && textNode.nodeValue) ? textNode.nodeValue : "";
}
function TreeView_PopulateNode(data, index, node, selectNode, selectImageNode, lineType, text, path, databound, datapath, parentIsLast) {
    if (!data) {
        return;
    }
    var context = new Object();
    context.data = data;
    context.node = node;
    context.selectNode = selectNode;
    context.selectImageNode = selectImageNode;
    context.lineType = lineType;
    context.index = index;
    context.isChecked = "f";
    var tr = WebForm_GetParentByTagName(node, "TR");
    if (tr) {
        var checkbox = tr.getElementsByTagName("INPUT");
        if (checkbox && (checkbox.length > 0)) {
            for (var i = 0; i < checkbox.length; i++) {
                if (checkbox[i].type.toLowerCase() == "checkbox") {
                    if (checkbox[i].checked) {
                        context.isChecked = "t";
                    }
                    break;
                }
            }
        }
    }
    var param = index + "|" + data.lastIndex + "|" + databound + context.isChecked + parentIsLast + "|" +
        text.length + "|" + text + datapath.length + "|" + datapath + path;
    TreeView_PopulateNodeDoCallBack(context, param);
}
function TreeView_ProcessNodeData(result, context) {
    var treeNode = context.node;
    if (result.length > 0) {
        var ci =  result.indexOf("|", 0);
        context.data.lastIndex = result.substring(0, ci);
        ci = result.indexOf("|", ci + 1);
        var newExpandState = result.substring(context.data.lastIndex.length + 1, ci);
        context.data.expandState.value += newExpandState;
        var chunk = result.substr(ci + 1);
        var newChildren, table;
        if (__nonMSDOMBrowser) {
            var newDiv = document.createElement("div");
            newDiv.innerHTML = chunk;
            table = WebForm_GetParentByTagName(treeNode, "TABLE");
            newChildren = null;
            if ((typeof(table.nextSibling) == "undefined") || (table.nextSibling == null)) {
                table.parentNode.insertBefore(newDiv.firstChild, table.nextSibling);
                newChildren = table.previousSibling;
            }
            else {
                table = table.nextSibling;
                table.parentNode.insertBefore(newDiv.firstChild, table);
                newChildren = table.previousSibling;
            }
            newChildren = document.getElementById(treeNode.id + "Nodes");
        }
        else {
            table = WebForm_GetParentByTagName(treeNode, "TABLE");
            table.insertAdjacentHTML("afterEnd", chunk);
            newChildren = document.all[treeNode.id + "Nodes"];
        }
        if ((typeof(newChildren) != "undefined") && (newChildren != null)) {
            TreeView_ToggleNode(context.data, context.index, treeNode, context.lineType, newChildren);
            treeNode.href = document.getElementById ?
                "javascript:TreeView_ToggleNode(" + context.data.name + "," + context.index + ",document.getElementById('" + treeNode.id + "'),'" + context.lineType + "',document.getElementById('" + newChildren.id + "'))" :
                "javascript:TreeView_ToggleNode(" + context.data.name + "," + context.index + "," + treeNode.id + ",'" + context.lineType + "'," + newChildren.id + ")";
            if ((typeof(context.selectNode) != "undefined") && (context.selectNode != null) && context.selectNode.href &&
                (context.selectNode.href.indexOf("javascript:TreeView_PopulateNode", 0) == 0)) {
                context.selectNode.href = treeNode.href;
            }
            if ((typeof(context.selectImageNode) != "undefined") && (context.selectImageNode != null) && context.selectNode.href &&
                (context.selectImageNode.href.indexOf("javascript:TreeView_PopulateNode", 0) == 0)) {
                context.selectImageNode.href = treeNode.href;
            }
        }
        context.data.populateLog.value += context.index + ",";
    }
    else {
        var img = treeNode.childNodes ? treeNode.childNodes[0] : treeNode.children[0];
        if ((typeof(img) != "undefined") && (img != null)) {
            var lineType = context.lineType;
            if (lineType == "l") {
                img.src = context.data.images[13];
            }
            else if (lineType == "t") {
                img.src = context.data.images[10];
            }
            else if (lineType == "-") {
                img.src = context.data.images[16];
            }
            else {
                img.src = context.data.images[3];
            }
            var pe;
            if (__nonMSDOMBrowser) {
                pe = treeNode.parentNode;
                pe.insertBefore(img, treeNode);
                pe.removeChild(treeNode);
            }
            else {
                pe = treeNode.parentElement;
                treeNode.style.visibility="hidden";
                treeNode.style.display="none";
                pe.insertAdjacentElement("afterBegin", img);
            }
        }
    }
}
function TreeView_SelectNode(data, node, nodeId) {
    if (!data) {
        return;
    }
    if ((typeof(data.selectedClass) != "undefined") && (data.selectedClass != null)) {
        var id = data.selectedNodeID.value;
        if (id.length > 0) {
            var selectedNode = document.getElementById(id);
            if ((typeof(selectedNode) != "undefined") && (selectedNode != null)) {
                WebForm_RemoveClassName(selectedNode, data.selectedHyperLinkClass);
                selectedNode = WebForm_GetParentByTagName(selectedNode, "TD");
                WebForm_RemoveClassName(selectedNode, data.selectedClass);
            }
        }
        WebForm_AppendToClassName(node, data.selectedHyperLinkClass);
        node = WebForm_GetParentByTagName(node, "TD");
        WebForm_AppendToClassName(node, data.selectedClass)
    }
    data.selectedNodeID.value = nodeId;
}
function TreeView_ToggleNode(data, index, node, lineType, children) {
    if (!data) {
        return;
    }
    var img = node.childNodes[0];
    var newExpandState;
    try {
        if (children.style.display == "none") {
            children.style.display = "block";
            newExpandState = "e";
            if ((typeof(img) != "undefined") && (img != null)) {
                if (lineType == "l") {
                    img.src = data.images[15];
                }
                else if (lineType == "t") {
                    img.src = data.images[12];
                }
                else if (lineType == "-") {
                    img.src = data.images[18];
                }
                else {
                    img.src = data.images[5];
                }
                img.alt = data.collapseToolTip.replace(/\{0\}/, TreeView_GetNodeText(node));
            }
        }
        else {
            children.style.display = "none";
            newExpandState = "c";
            if ((typeof(img) != "undefined") && (img != null)) {
                if (lineType == "l") {
                    img.src = data.images[14];
                }
                else if (lineType == "t") {
                    img.src = data.images[11];
                }
                else if (lineType == "-") {
                    img.src = data.images[17];
                }
                else {
                    img.src = data.images[4];
                }
                img.alt = data.expandToolTip.replace(/\{0\}/, TreeView_GetNodeText(node));
            }
        }
    }
    catch(e) {}
    data.expandState.value =  data.expandState.value.substring(0, index) + newExpandState + data.expandState.value.slice(index + 1);
}
function TreeView_UnhoverNode(node) {
    if (!node.hoverClass) {
        return;
    }
    WebForm_RemoveClassName(node, node.hoverClass);
    if (__nonMSDOMBrowser) {
        node = node.childNodes[node.childNodes.length - 1];
    }
    else {
        node = node.children[node.children.length - 1];
    }
    WebForm_RemoveClassName(node, node.hoverHyperLinkClass);
}
