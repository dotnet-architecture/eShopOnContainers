//CdnPath=http://ajax.aspnetcdn.com/ajax/4.5.1/1/SmartNav.js
var snSrc;
if ((typeof(window.__smartNav) == "undefined") || (window.__smartNav == null))
{
    window.__smartNav = new Object();
    window.__smartNav.update = function()
    {
        var sn = window.__smartNav;
        var fd;
        document.detachEvent("onstop", sn.stopHif);
        sn.inPost = false;
        try { fd = frames["__hifSmartNav"].document; } catch (e) {return;}
        var fdr = fd.getElementsByTagName("asp_smartnav_rdir");
        if (fdr.length > 0)
        {
            if ((typeof(sn.sHif) == "undefined") || (sn.sHif == null))
            {
                sn.sHif = document.createElement("IFRAME");
                sn.sHif.name = "__hifSmartNav";
                sn.sHif.style.display = "none";
                sn.sHif.src = snSrc;
            }
            try {window.location = fdr[0].url;} catch (e) {};
            return;
        }
        var fdurl = fd.location.href;
        var index = fdurl.indexOf(snSrc);
        if ((index != -1 && index == fdurl.length-snSrc.length)
            || fdurl == "about:blank")
            return;
		var fdurlb = fdurl.split("?")[0];
		if (document.location.href.indexOf(fdurlb) < 0)
		{
            document.location.href=fdurl;
		    return;
		}
		sn._savedOnLoad = window.onload;
		window.onload = null;
		window.__smartNav.updateHelper();
	}
	window.__smartNav.updateHelper = function()
	{
		if (document.readyState != "complete")
		{
		    window.setTimeout(window.__smartNav.updateHelper, 25);
		    return;
		}
		window.__smartNav.loadNewContent();
	}
	window.__smartNav.loadNewContent = function()
	{
		var sn = window.__smartNav;
		var fd;
		try { fd = frames["__hifSmartNav"].document; } catch (e) {return;}
        if ((typeof(sn.sHif) != "undefined") && (sn.sHif != null))
        {
            sn.sHif.removeNode(true);
            sn.sHif = null;
        }
        var hdm = document.getElementsByTagName("head")[0];
        var hk = hdm.childNodes;
        var tt = null;
        var i;
        for (i = hk.length - 1; i>= 0; i--)
        {
            if (hk[i].tagName == "TITLE")
            {
                tt = hk[i].outerHTML;
                continue;
            }
            if (hk[i].tagName != "BASEFONT" || hk[i].innerHTML.length == 0)
                hdm.removeChild(hdm.childNodes[i]);
        }
        var kids = fd.getElementsByTagName("head")[0].childNodes;
        for (i = 0; i < kids.length; i++)
        {
            var tn = kids[i].tagName;
            var k = document.createElement(tn);
            k.id = kids[i].id;
            k.mergeAttributes(kids[i]);
            switch(tn)
            {
            case "TITLE":
                if (tt == kids[i].outerHTML)
                    continue;
                k.innerText = kids[i].text;
                hdm.insertAdjacentElement("afterbegin", k);
                continue;
            case "BASEFONT" :
                if (kids[i].innerHTML.length > 0)
                    continue;
                break;
            default:
                var o = document.createElement("BODY");
                o.innerHTML = "<BODY>" + kids[i].outerHTML + "</BODY>";
                k = o.firstChild;
                break;
            }
            if((typeof(k) != "undefined") && (k != null))
                hdm.appendChild(k);
        }
        document.body.clearAttributes();
        document.body.id = fd.body.id;
        document.body.mergeAttributes(fd.body);
        var newBodyLoad = fd.body.onload;
        if ((typeof(newBodyLoad) != "undefined") && (newBodyLoad != null))
            document.body.onload = newBodyLoad;
        else
            document.body.onload = sn._savedOnLoad;
        var s = "<BODY>" + fd.body.innerHTML + "</BODY>";
        if ((typeof(sn.hif) != "undefined") && (sn.hif != null))
        {
            var hifP = sn.hif.parentElement;
            if ((typeof(hifP) != "undefined") && (hifP != null))
                sn.sHif=hifP.removeChild(sn.hif);
        }
        document.body.innerHTML = s;
        var sc = document.scripts;
        for (i = 0; i < sc.length; i++)
        {
            sc[i].text = sc[i].text;
        }
        sn.hif = document.all("__hifSmartNav");
        if ((typeof(sn.hif) != "undefined") && (sn.hif != null))
        {
            var hif = sn.hif;
            sn.hifName = "__hifSmartNav" + (new Date()).getTime();
            frames["__hifSmartNav"].name = sn.hifName;
            sn.hifDoc = hif.contentWindow.document;
            if (sn.ie5)
                hif.parentElement.removeChild(hif);
            window.setTimeout(sn.restoreFocus,0);
        }
        if (typeof(window.onload) == "string")
        {
            try { eval(window.onload) } catch (e) {};
        }
        else if ((typeof(window.onload) != "undefined") && (window.onload != null))
        {
            try { window.onload() } catch (e) {};
        }
        sn._savedOnLoad = null;
        sn.attachForm();
    };
    window.__smartNav.restoreFocus = function()
    {
        if (window.__smartNav.inPost == true) return;
        var curAe = document.activeElement;
        var sAeId = window.__smartNav.ae;
        if (((typeof(sAeId) == "undefined") || (sAeId == null)) ||
            (typeof(curAe) != "undefined") && (curAe != null) && (curAe.id == sAeId || curAe.name == sAeId))
            return;
        var ae = document.all(sAeId);
        if ((typeof(ae) == "undefined") || (ae == null)) return;
        try { ae.focus(); } catch(e){};
    }
    window.__smartNav.saveHistory = function()
    {
        if ((typeof(window.__smartNav.hif) != "undefined") && (window.__smartNav.hif != null))
            window.__smartNav.hif.removeNode();
        if ((typeof(window.__smartNav.sHif) != "undefined") && (window.__smartNav.sHif != null)
            && (typeof(document.all[window.__smartNav.siHif]) != "undefined")
            && (document.all[window.__smartNav.siHif] != null)) {
            document.all[window.__smartNav.siHif].insertAdjacentElement(
                        "BeforeBegin", window.__smartNav.sHif);
        }
    }
    window.__smartNav.stopHif = function()
    {
        document.detachEvent("onstop", window.__smartNav.stopHif);
        var sn = window.__smartNav;
        if (((typeof(sn.hifDoc) == "undefined") || (sn.hifDoc == null)) &&
            (typeof(sn.hif) != "undefined") && (sn.hif != null))
        {
            try {sn.hifDoc = sn.hif.contentWindow.document;}
            catch(e){sn.hifDoc=null}
        }
        if (sn.hifDoc != null)
        {
            try {sn.hifDoc.execCommand("stop");} catch (e){}
        }
    }
    window.__smartNav.init =  function()
    {
        var sn = window.__smartNav;
        window.__smartNav.form.__smartNavPostBack.value = 'true';
        document.detachEvent("onstop", sn.stopHif);
        document.attachEvent("onstop", sn.stopHif);
        try { if (window.event.returnValue == false) return; } catch(e) {}
        sn.inPost = true;
        if ((typeof(document.activeElement) != "undefined") && (document.activeElement != null))
        {
            var ae = document.activeElement.id;
            if (ae.length == 0)
                ae = document.activeElement.name;
            sn.ae = ae;
        }
        else
            sn.ae = null;
        try {document.selection.empty();} catch (e) {}
        if ((typeof(sn.hif) == "undefined") || (sn.hif == null))
        {
            sn.hif = document.all("__hifSmartNav");
            sn.hifDoc = sn.hif.contentWindow.document;
        }
        if ((typeof(sn.hifDoc) != "undefined") && (sn.hifDoc != null))
            try {sn.hifDoc.designMode = "On";} catch(e){};
        if ((typeof(sn.hif.parentElement) == "undefined") || (sn.hif.parentElement == null))
            document.body.appendChild(sn.hif);
        var hif = sn.hif;
        hif.detachEvent("onload", sn.update);
        hif.attachEvent("onload", sn.update);
        window.__smartNav.fInit = true;
    };
    window.__smartNav.submit = function()
    {
        window.__smartNav.fInit = false;
        try { window.__smartNav.init(); } catch(e) {}
        if (window.__smartNav.fInit) {
            window.__smartNav.form._submit();
        }
    };
    window.__smartNav.attachForm = function()
    {
        var cf = document.forms;
        for (var i=0; i<cf.length; i++)
        {
            if ((typeof(cf[i].__smartNavEnabled) != "undefined") && (cf[i].__smartNavEnabled != null))
            {
                window.__smartNav.form = cf[i];
                window.__smartNav.form.insertAdjacentHTML("beforeEnd", "<input type='hidden' name='__smartNavPostBack' value='false' />");
                break;
            }
        }
        var snfm = window.__smartNav.form;
        if ((typeof(snfm) == "undefined") || (snfm == null)) return false;
        var sft = snfm.target;
        if (sft.length != 0 && sft.indexOf("__hifSmartNav") != 0) return false;
        var sfc = snfm.action.split("?")[0];
        var url = window.location.href.split("?")[0];
        if (url.charAt(url.length-1) != '/' && url.lastIndexOf(sfc) + sfc.length != url.length) return false;
        if (snfm.__formAttached == true) return true;
        snfm.__formAttached = true;
        snfm.attachEvent("onsubmit", window.__smartNav.init);
        snfm._submit = snfm.submit;
        snfm.submit = window.__smartNav.submit;
        snfm.target = window.__smartNav.hifName;
        return true;
    };
    window.__smartNav.hifName = "__hifSmartNav" + (new Date()).getTime();
    window.__smartNav.ie5 = navigator.appVersion.indexOf("MSIE 5") > 0;
    var rc = window.__smartNav.attachForm();
    var hif = document.all("__hifSmartNav");
    if ((typeof(snSrc) == "undefined") || (snSrc == null)) {
	    if (typeof(window.dialogHeight) != "undefined") {
	            snSrc = "IEsmartnav1";
		    hif.src = snSrc;
	    } else {
		    snSrc = hif.src;
	    }
    }
    if (rc)
    {
        var fsn = frames["__hifSmartNav"];
        fsn.name = window.__smartNav.hifName;
        window.__smartNav.siHif = hif.sourceIndex;
        try {
            if (fsn.document.location != snSrc)
            {
                fsn.document.designMode = "On";
                hif.attachEvent("onload",window.__smartNav.update);
                window.__smartNav.hif = hif;
            }
        }
        catch (e) { window.__smartNav.hif = hif; }
        window.attachEvent("onbeforeunload", window.__smartNav.saveHistory);
    }
    else
        window.__smartNav = null;
}
