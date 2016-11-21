import { Component, OnInit, Output, Input, EventEmitter } from '@angular/core';

import { IPager } from '../../models/pager.model';

@Component({
    selector: 'esh-pager',
    templateUrl: './pager.html',
    styleUrls: ['./pager.scss']
})
export class Pager implements OnInit {

    @Output()
    changed: EventEmitter<number> = new EventEmitter<number>();

    @Input()
    model: IPager;

    ngOnInit() {
        console.log(this.model);
    }

    onNextClicked(event: any) {
        event.preventDefault();
        console.log('Pager Next Clicked');
        this.changed.emit(this.model.actualPage + 1);
    }

    onPreviousCliked(event: any) {
        event.preventDefault();
        this.changed.emit(this.model.actualPage - 1);
    }

}
