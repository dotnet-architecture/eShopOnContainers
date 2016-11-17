import { Component, Input } from '@angular/core';

@Component({
    selector: 'appc-page-heading',
    template: `<h4>{{text}}</h4>`
})
export class PageHeadingComponent {
    @Input() text: string;
    constructor() { }
}
