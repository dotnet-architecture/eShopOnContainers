//CdnPath=http://ajax.aspnetcdn.com/ajax/4.5.1/1/MenuStandards.js
if (!window.Sys) { window.Sys = {}; }
if (!Sys.WebForms) { Sys.WebForms = {}; }
Sys.WebForms.Menu = function(options) {
    this.items = [];
    this.depth = options.depth || 1;
    this.parentMenuItem = options.parentMenuItem;
    this.element = Sys.WebForms.Menu._domHelper.getElement(options.element);
    if (this.element.tagName === 'DIV') {
        var containerElement = this.element;
        this.element = Sys.WebForms.Menu._domHelper.firstChild(containerElement);
        this.element.tabIndex = options.tabIndex || 0;
        options.element = containerElement;
        options.menu = this;
        this.container = new Sys.WebForms._MenuContainer(options);
        Sys.WebForms.Menu._domHelper.setFloat(this.element, this.container.rightToLeft ? "right" : "left");
    }
    else {
        this.container = options.container;
        this.keyMap = options.keyMap;
    }
    Sys.WebForms.Menu._elementObjectMapper.map(this.element, this);
    if (this.parentMenuItem && this.parentMenuItem.parentMenu) {
        this.parentMenu = this.parentMenuItem.parentMenu;
        this.rootMenu = this.parentMenu.rootMenu;
        if (!this.element.id) {
            this.element.id = (this.container.element.id || 'menu') + ':submenu:' + Sys.WebForms.Menu._elementObjectMapper._computedId;
        }
        if (this.depth > this.container.staticDisplayLevels) {
            this.displayMode = "dynamic";
            this.element.style.display = "none";
            this.element.style.position = "absolute";
            if (this.rootMenu && this.container.orientation === 'horizontal' && this.parentMenu.isStatic()) {
                this.element.style.top = "100%";
                if (this.container.rightToLeft) {
                    this.element.style.right = "0px";
                }
                else {
                    this.element.style.left = "0px";
                }
            }
            else {
                this.element.style.top = "0px";
                if (this.container.rightToLeft) {
                    this.element.style.right = "100%";
                }
                else {
                    this.element.style.left = "100%";
                }
            }
            if (this.container.rightToLeft) {
                this.keyMap = Sys.WebForms.Menu._keyboardMapping.verticalRtl;
            }
            else {
                this.keyMap = Sys.WebForms.Menu._keyboardMapping.vertical;
            }
        }
        else {
            this.displayMode = "static";
            this.element.style.display = "block";
            if (this.container.orientation === 'horizontal') {
                Sys.WebForms.Menu._domHelper.setFloat(this.element, this.container.rightToLeft ? "right" : "left");
            }
        }
    }
    Sys.WebForms.Menu._domHelper.appendCssClass(this.element, this.displayMode);
    var children = this.element.childNodes;
    var count = children.length;
    for (var i = 0; i < count; i++) {
        var node = children[i];
        if (node.nodeType !== 1) {   
            continue;
        }
        var topLevelMenuItem = null;
        if (this.parentMenuItem) {
            topLevelMenuItem = this.parentMenuItem.topLevelMenuItem;
        }
        var menuItem = new Sys.WebForms.MenuItem(this, node, topLevelMenuItem);
        var previousMenuItem = this.items[this.items.length - 1];
        if (previousMenuItem) {
            menuItem.previousSibling = previousMenuItem;
            previousMenuItem.nextSibling = menuItem;
        }
        this.items[this.items.length] = menuItem;
    }
};
Sys.WebForms.Menu.prototype = {
    blur: function() { if (this.container) this.container.blur(); },
    collapse: function() {
        this.each(function(menuItem) {
            menuItem.hover(false);
            menuItem.blur();
            var childMenu = menuItem.childMenu;
            if (childMenu) {
                childMenu.collapse();
            }
        });
        this.hide();
    },
    doDispose: function() { this.each(function(item) { item.doDispose(); }); },
    each: function(fn) {
        var count = this.items.length;
        for (var i = 0; i < count; i++) {
            fn(this.items[i]);
        }
    },
    firstChild: function() { return this.items[0]; },
    focus: function() { if (this.container) this.container.focus(); },
    get_displayed: function() { return this.element.style.display !== 'none'; },
    get_focused: function() {
        if (this.container) {
            return this.container.focused;
        }
        return false;
    },
    handleKeyPress: function(keyCode) {
        if (this.keyMap.contains(keyCode)) {
            if (this.container.focusedMenuItem) {
                this.container.focusedMenuItem.navigate(keyCode);
                return;
            }
            var firstChild = this.firstChild();
            if (firstChild) {
                this.container.navigateTo(firstChild);
            }
        }
    },
    hide: function() {
        if (!this.get_displayed()) {
            return;
        }
        this.each(function(item) {
            if (item.childMenu) {
                item.childMenu.hide();
            }
        });
        if (!this.isRoot()) {
            if (this.get_focused()) {
                this.container.navigateTo(this.parentMenuItem);
            }
            this.element.style.display = 'none';
        }
    },
    isRoot: function() { return this.rootMenu === this; },
    isStatic: function() { return this.displayMode === 'static'; },
    lastChild: function() { return this.items[this.items.length - 1]; },
    show: function() { this.element.style.display = 'block'; }
};
if (Sys.WebForms.Menu.registerClass) {
    Sys.WebForms.Menu.registerClass('Sys.WebForms.Menu');
}
Sys.WebForms.MenuItem = function(parentMenu, listElement, topLevelMenuItem) {
    this.keyMap = parentMenu.keyMap;
    this.parentMenu = parentMenu;
    this.container = parentMenu.container;
    this.element = listElement;
    this.topLevelMenuItem = topLevelMenuItem || this;
    this._anchor = Sys.WebForms.Menu._domHelper.firstChild(listElement);
    while (this._anchor && this._anchor.tagName !== 'A') {
        this._anchor = Sys.WebForms.Menu._domHelper.nextSibling(this._anchor);
    }
    if (this._anchor) {
        this._anchor.tabIndex = -1;
        var subMenu = this._anchor;
        while (subMenu && subMenu.tagName !== 'UL') {
            subMenu = Sys.WebForms.Menu._domHelper.nextSibling(subMenu);
        }
        if (subMenu) {
            this.childMenu = new Sys.WebForms.Menu({ element: subMenu, parentMenuItem: this, depth: parentMenu.depth + 1, container: this.container, keyMap: this.keyMap });
            if (!this.childMenu.isStatic()) {
                Sys.WebForms.Menu._domHelper.appendCssClass(this.element, 'has-popup');
                Sys.WebForms.Menu._domHelper.appendAttributeValue(this.element, 'aria-haspopup', this.childMenu.element.id);
            }
        }
    }
    Sys.WebForms.Menu._elementObjectMapper.map(listElement, this);
    Sys.WebForms.Menu._domHelper.appendAttributeValue(listElement, 'role', 'menuitem');
    Sys.WebForms.Menu._domHelper.appendCssClass(listElement, parentMenu.displayMode);
    if (this._anchor) {
        Sys.WebForms.Menu._domHelper.appendCssClass(this._anchor, parentMenu.displayMode);
    }
    this.element.style.position = "relative";
    if (this.parentMenu.depth == 1 && this.container.orientation == 'horizontal') {
        Sys.WebForms.Menu._domHelper.setFloat(this.element, this.container.rightToLeft ? "right" : "left");
    }
    if (!this.container.disabled) {
        Sys.WebForms.Menu._domHelper.addEvent(this.element, 'mouseover', Sys.WebForms.MenuItem._onmouseover);
        Sys.WebForms.Menu._domHelper.addEvent(this.element, 'mouseout', Sys.WebForms.MenuItem._onmouseout);
    }
};
Sys.WebForms.MenuItem.prototype = {
    applyUp: function(fn, condition) {
        condition = condition || function(menuItem) { return menuItem; };
        var menuItem = this;
        var lastMenuItem = null;
        while (condition(menuItem)) {
            fn(menuItem);
            lastMenuItem = menuItem;
            menuItem = menuItem.parentMenu.parentMenuItem;
        }
        return lastMenuItem;
    },
    blur: function() { this.setTabIndex(-1); },
    doDispose: function() {
        Sys.WebForms.Menu._domHelper.removeEvent(this.element, 'mouseover', Sys.WebForms.MenuItem._onmouseover);
        Sys.WebForms.Menu._domHelper.removeEvent(this.element, 'mouseout', Sys.WebForms.MenuItem._onmouseout);
        if (this.childMenu) {
            this.childMenu.doDispose();
        }
    },
    focus: function() {
        if (!this.parentMenu.get_displayed()) {
            this.parentMenu.show();
        }
        this.setTabIndex(0);
        this.container.focused = true;
        this._anchor.focus();
    },
    get_highlighted: function() { return /(^|\s)highlighted(\s|$)/.test(this._anchor.className); },
    getTabIndex: function() { return this._anchor.tabIndex; },
    highlight: function(highlighting) {
        if (highlighting) {
            this.applyUp(function(menuItem) {
                menuItem.parentMenu.parentMenuItem.highlight(true);
            },
            function(menuItem) {
                return !menuItem.parentMenu.isStatic() && menuItem.parentMenu.parentMenuItem;
            }
        );
            Sys.WebForms.Menu._domHelper.appendCssClass(this._anchor, 'highlighted');
        }
        else {
            Sys.WebForms.Menu._domHelper.removeCssClass(this._anchor, 'highlighted');
            this.setTabIndex(-1);
        }
    },
    hover: function(hovering) {
        if (hovering) {
            var currentHoveredItem = this.container.hoveredMenuItem;
            if (currentHoveredItem) {
                currentHoveredItem.hover(false);
            }
            var currentFocusedItem = this.container.focusedMenuItem;
            if (currentFocusedItem && currentFocusedItem !== this) {
                currentFocusedItem.hover(false);
            }
            this.applyUp(function(menuItem) {
                if (menuItem.childMenu && !menuItem.childMenu.get_displayed()) {
                    menuItem.childMenu.show();
                }
            });
            this.container.hoveredMenuItem = this;
            this.highlight(true);
        }
        else {
            var menuItem = this;
            while (menuItem) {
                menuItem.highlight(false);
                if (menuItem.childMenu) {
                    if (!menuItem.childMenu.isStatic()) {
                        menuItem.childMenu.hide();
                    }
                }
                menuItem = menuItem.parentMenu.parentMenuItem;
            }
        }
    },
    isSiblingOf: function(menuItem) { return menuItem.parentMenu === this.parentMenu; },
    mouseout: function() {
        var menuItem = this,
            id = this.container.pendingMouseoutId,
            disappearAfter = this.container.disappearAfter;
        if (id) {
            window.clearTimeout(id);
        }
        if (disappearAfter > -1) {
            this.container.pendingMouseoutId =
                window.setTimeout(function() { menuItem.hover(false); }, disappearAfter);
        }
    },
    mouseover: function() {
        var id = this.container.pendingMouseoutId;
        if (id) {
            window.clearTimeout(id);
            this.container.pendingMouseoutId = null;
        }
        this.hover(true);
        if (this.container.menu.get_focused()) {
            this.container.navigateTo(this);
        }
    },
    navigate: function(keyCode) {
        switch (this.keyMap[keyCode]) {
            case this.keyMap.next:
                this.navigateNext();
                break;
            case this.keyMap.previous:
                this.navigatePrevious();
                break;
            case this.keyMap.child:
                this.navigateChild();
                break;
            case this.keyMap.parent:
                this.navigateParent();
                break;
            case this.keyMap.tab:
                this.navigateOut();
                break;
        }
    },
    navigateChild: function() {
        var subMenu = this.childMenu;
        if (subMenu) {
            var firstChild = subMenu.firstChild();
            if (firstChild) {
                this.container.navigateTo(firstChild);
            }
        }
        else {
            if (this.container.orientation === 'horizontal') {
                var nextItem = this.topLevelMenuItem.nextSibling || this.topLevelMenuItem.parentMenu.firstChild();
                if (nextItem == this.topLevelMenuItem) {
                    return;
                }
                this.topLevelMenuItem.childMenu.hide();
                this.container.navigateTo(nextItem);
                if (nextItem.childMenu) {
                    this.container.navigateTo(nextItem.childMenu.firstChild());
                }
            }
        }
    },
    navigateNext: function() {
        if (this.childMenu) {
            this.childMenu.hide();
        }
        var nextMenuItem = this.nextSibling;
        if (!nextMenuItem && this.parentMenu.isRoot()) {
            nextMenuItem = this.parentMenu.parentMenuItem;
            if (nextMenuItem) {
                nextMenuItem = nextMenuItem.nextSibling;
            }
        }
        if (!nextMenuItem) {
            nextMenuItem = this.parentMenu.firstChild();
        }
        if (nextMenuItem) {
            this.container.navigateTo(nextMenuItem);
        }
    },
    navigateOut: function() {
        this.parentMenu.blur();
    },
    navigateParent: function() {
        var parentMenu = this.parentMenu,
            horizontal = this.container.orientation === 'horizontal';
        if (!parentMenu) return;
        if (horizontal && this.childMenu && parentMenu.isRoot()) {
            this.navigateChild();
            return;
        }
        if (parentMenu.parentMenuItem && !parentMenu.isRoot()) {
            if (horizontal && this.parentMenu.depth === 2) {
                var previousItem = this.parentMenu.parentMenuItem.previousSibling;
                if (!previousItem) {
                    previousItem = this.parentMenu.rootMenu.lastChild();
                }
                this.topLevelMenuItem.childMenu.hide();
                this.container.navigateTo(previousItem);
                if (previousItem.childMenu) {
                    this.container.navigateTo(previousItem.childMenu.firstChild());
                }
            }
            else {
                this.parentMenu.hide();
            }
        }
    },
    navigatePrevious: function() {
        if (this.childMenu) {
            this.childMenu.hide();
        }
        var previousMenuItem = this.previousSibling;
        if (previousMenuItem) {
            var childMenu = previousMenuItem.childMenu;
            if (childMenu && childMenu.isRoot()) {
                previousMenuItem = childMenu.lastChild();
            }
        }
        if (!previousMenuItem && this.parentMenu.isRoot()) {
            previousMenuItem = this.parentMenu.parentMenuItem;
        }
        if (!previousMenuItem) {
            previousMenuItem = this.parentMenu.lastChild();
        }
        if (previousMenuItem) {
            this.container.navigateTo(previousMenuItem);
        }
    },
    setTabIndex: function(index) { if (this._anchor) this._anchor.tabIndex = index; }
};
Sys.WebForms.MenuItem._onmouseout = function(e) {
    var menuItem = Sys.WebForms.Menu._elementObjectMapper.getMappedObject(this);
    if (!menuItem) {
        return;
    }
    menuItem.mouseout();
    Sys.WebForms.Menu._domHelper.cancelEvent(e);
};
Sys.WebForms.MenuItem._onmouseover = function(e) {
    var menuItem = Sys.WebForms.Menu._elementObjectMapper.getMappedObject(this);
    if (!menuItem) {
        return;
    }
    menuItem.mouseover();
    Sys.WebForms.Menu._domHelper.cancelEvent(e);
};
Sys.WebForms.Menu._domHelper = {
    addEvent: function(element, eventName, fn, useCapture) {
        if (element.addEventListener) {
            element.addEventListener(eventName, fn, !!useCapture);
        }
        else {
            element['on' + eventName] = fn;
        }
    },
    appendAttributeValue: function(element, name, value) {
        this.updateAttributeValue('append', element, name, value);
    },
    appendCssClass: function(element, value) {
        this.updateClassName('append', element, name, value);
    },
    appendString: function(getString, setString, value) {
        var currentValue = getString();
        if (!currentValue) {
            setString(value);
            return;
        }
        var regex = this._regexes.getRegex('(^| )' + value + '($| )');
        if (regex.test(currentValue)) {
            return;
        }
        setString(currentValue + ' ' + value);
    },
    cancelEvent: function(e) {
        var event = e || window.event;
        if (event) {
            event.cancelBubble = true;
            if (event.stopPropagation) {
                event.stopPropagation();
            }
        }
    },
    contains: function(ancestor, descendant) {
        for (; descendant && (descendant !== ancestor); descendant = descendant.parentNode) { }
        return !!descendant;
    },
    firstChild: function(element) {
        var child = element.firstChild;
        if (child && child.nodeType !== 1) {   
            child = this.nextSibling(child);
        }
        return child;
    },
    getElement: function(elementOrId) { return typeof elementOrId === 'string' ? document.getElementById(elementOrId) : elementOrId; },
    getElementDirection: function(element) {
        if (element) {
            if (element.dir) {
                return element.dir;
            }
            return this.getElementDirection(element.parentNode);
        }
        return "ltr";
    },
    getKeyCode: function(event) { return event.keyCode || event.charCode || 0; },
    insertAfter: function(element, elementToInsert) {
        var next = element.nextSibling;
        if (next) {
            element.parentNode.insertBefore(elementToInsert, next);
        }
        else if (element.parentNode) {
            element.parentNode.appendChild(elementToInsert);
        }
    },
    nextSibling: function(element) {
        var sibling = element.nextSibling;
        while (sibling) {
            if (sibling.nodeType === 1) {   
                return sibling;
            }
            sibling = sibling.nextSibling;
        }
    },
    removeAttributeValue: function(element, name, value) {
        this.updateAttributeValue('remove', element, name, value);
    },
    removeCssClass: function(element, value) {
        this.updateClassName('remove', element, name, value);
    },
    removeEvent: function(element, eventName, fn, useCapture) {
        if (element.removeEventListener) {
            element.removeEventListener(eventName, fn, !!useCapture);
        }
        else if (element.detachEvent) {
            element.detachEvent('on' + eventName, fn)
        }
        element['on' + eventName] = null;
    },
    removeString: function(getString, setString, valueToRemove) {
        var currentValue = getString();
        if (currentValue) {
            var regex = this._regexes.getRegex('(\\s|\\b)' + valueToRemove + '$|\\b' + valueToRemove + '\\s+');
            setString(currentValue.replace(regex, ''));
        }
    },
    setFloat: function(element, direction) {
        element.style.styleFloat = direction;
        element.style.cssFloat = direction;
    },
    updateAttributeValue: function(operation, element, name, value) {
        this[operation + 'String'](
                function() {
                    return element.getAttribute(name);
                },
                function(newValue) {
                    element.setAttribute(name, newValue);
                },
                value
            );
    },
    updateClassName: function(operation, element, name, value) {
        this[operation + 'String'](
                function() {
                    return element.className;
                },
                function(newValue) {
                    element.className = newValue;
                },
                value
            );
    },
    _regexes: {
        getRegex: function(pattern) {
            var regex = this[pattern];
            if (!regex) {
                this[pattern] = regex = new RegExp(pattern);
            }
            return regex;
        }
    }
};
Sys.WebForms.Menu._elementObjectMapper = {
    _computedId: 0,
    _mappings: {},
    _mappingIdName: 'Sys.WebForms.Menu.Mapping',
    getMappedObject: function(element) {
        var id = element[this._mappingIdName];
        if (id) {
            return this._mappings[this._mappingIdName + ':' + id];
        }
    },
    map: function(element, theObject) {
        var mappedObject = element[this._mappingIdName];
        if (mappedObject === theObject) {
            return;
        }
        var objectId = element[this._mappingIdName] || element.id || '%' + (++this._computedId); 
        element[this._mappingIdName] = objectId;
        this._mappings[this._mappingIdName + ':' + objectId] = theObject;
        theObject.mappingId = objectId;
    }
};
Sys.WebForms.Menu._keyboardMapping = new (function() {
    var LEFT_ARROW = 37;
    var UP_ARROW = 38;
    var RIGHT_ARROW = 39;
    var DOWN_ARROW = 40;
    var TAB = 9;
    var ESCAPE = 27;
    this.vertical = { next: 0, previous: 1, child: 2, parent: 3, tab: 4 };
    this.vertical[DOWN_ARROW] = this.vertical.next;
    this.vertical[UP_ARROW] = this.vertical.previous;
    this.vertical[RIGHT_ARROW] = this.vertical.child;
    this.vertical[LEFT_ARROW] = this.vertical.parent;
    this.vertical[TAB] = this.vertical[ESCAPE] = this.vertical.tab;
    this.verticalRtl = { next: 0, previous: 1, child: 2, parent: 3, tab: 4 };
    this.verticalRtl[DOWN_ARROW] = this.verticalRtl.next;
    this.verticalRtl[UP_ARROW] = this.verticalRtl.previous;
    this.verticalRtl[LEFT_ARROW] = this.verticalRtl.child;
    this.verticalRtl[RIGHT_ARROW] = this.verticalRtl.parent;
    this.verticalRtl[TAB] = this.verticalRtl[ESCAPE] = this.verticalRtl.tab;
    this.horizontal = { next: 0, previous: 1, child: 2, parent: 3, tab: 4 };
    this.horizontal[RIGHT_ARROW] = this.horizontal.next;
    this.horizontal[LEFT_ARROW] = this.horizontal.previous;
    this.horizontal[DOWN_ARROW] = this.horizontal.child;
    this.horizontal[UP_ARROW] = this.horizontal.parent;
    this.horizontal[TAB] = this.horizontal[ESCAPE] = this.horizontal.tab;
    this.horizontalRtl = { next: 0, previous: 1, child: 2, parent: 3, tab: 4 };
    this.horizontalRtl[RIGHT_ARROW] = this.horizontalRtl.previous;
    this.horizontalRtl[LEFT_ARROW] = this.horizontalRtl.next;
    this.horizontalRtl[DOWN_ARROW] = this.horizontalRtl.child;
    this.horizontalRtl[UP_ARROW] = this.horizontalRtl.parent;
    this.horizontalRtl[TAB] = this.horizontalRtl[ESCAPE] = this.horizontalRtl.tab;
    this.horizontal.contains = this.horizontalRtl.contains = this.vertical.contains = this.verticalRtl.contains = function(keycode) {
        return this[keycode] != null;
    };
})();
Sys.WebForms._MenuContainer = function(options) {
    this.focused = false;
    this.disabled = options.disabled;
    this.staticDisplayLevels = options.staticDisplayLevels || 1;
    this.element = options.element;
    this.orientation = options.orientation || 'vertical';
    this.disappearAfter = options.disappearAfter;
    this.rightToLeft = Sys.WebForms.Menu._domHelper.getElementDirection(this.element) === 'rtl';
    Sys.WebForms.Menu._elementObjectMapper.map(this.element, this);
    this.menu = options.menu;
    this.menu.rootMenu = this.menu;
    this.menu.displayMode = 'static';
    this.menu.element.style.position = 'relative';
    this.menu.element.style.width = 'auto';
    if (this.orientation === 'vertical') {
        Sys.WebForms.Menu._domHelper.appendAttributeValue(this.menu.element, 'role', 'menu');
        if (this.rightToLeft) {
            this.menu.keyMap = Sys.WebForms.Menu._keyboardMapping.verticalRtl;
        }
        else {
            this.menu.keyMap = Sys.WebForms.Menu._keyboardMapping.vertical;
        }
    }
    else {
        Sys.WebForms.Menu._domHelper.appendAttributeValue(this.menu.element, 'role', 'menubar');
        if (this.rightToLeft) {
            this.menu.keyMap = Sys.WebForms.Menu._keyboardMapping.horizontalRtl;
        }
        else {
            this.menu.keyMap = Sys.WebForms.Menu._keyboardMapping.horizontal;
        }
    }
    var floatBreak = document.createElement('div');
    floatBreak.style.clear = this.rightToLeft ? "right" : "left";
    this.element.appendChild(floatBreak);
    Sys.WebForms.Menu._domHelper.setFloat(this.element, this.rightToLeft ? "right" : "left");
    Sys.WebForms.Menu._domHelper.insertAfter(this.element, floatBreak);
    if (!this.disabled) {
        Sys.WebForms.Menu._domHelper.addEvent(this.menu.element, 'focus', this._onfocus, true);
        Sys.WebForms.Menu._domHelper.addEvent(this.menu.element, 'keydown', this._onkeydown);
        var menuContainer = this;
        this.element.dispose = function() {
            if (menuContainer.element.dispose) {
                menuContainer.element.dispose = null;
                Sys.WebForms.Menu._domHelper.removeEvent(menuContainer.menu.element, 'focus', menuContainer._onfocus, true);
                Sys.WebForms.Menu._domHelper.removeEvent(menuContainer.menu.element, 'keydown', menuContainer._onkeydown);
                menuContainer.menu.doDispose();
            }
        };
        Sys.WebForms.Menu._domHelper.addEvent(window, 'unload', function() {
            if (menuContainer.element.dispose) {
                menuContainer.element.dispose();
            }
        });
    }
};
Sys.WebForms._MenuContainer.prototype = {
    blur: function() {
        this.focused = false;
        this.isBlurring = false;
        this.menu.collapse();
        this.focusedMenuItem = null;
    },
    focus: function(e) { this.focused = true; },
    navigateTo: function(menuItem) {
        if (this.focusedMenuItem && this.focusedMenuItem !== this) {
            this.focusedMenuItem.highlight(false);
        }
        menuItem.highlight(true);
        menuItem.focus();
        this.focusedMenuItem = menuItem;
    },
    _onfocus: function(e) {
        var event = e || window.event;
        if (event.srcElement && this) {
            if (Sys.WebForms.Menu._domHelper.contains(this.element, event.srcElement)) {
                if (!this.focused) {
                    this.focus();
                }
            }
        }
    },
    _onkeydown: function(e) {
        var thisMenu = Sys.WebForms.Menu._elementObjectMapper.getMappedObject(this);
        var keyCode = Sys.WebForms.Menu._domHelper.getKeyCode(e || window.event);
        if (thisMenu) {
            thisMenu.handleKeyPress(keyCode);
        }
    }
};
