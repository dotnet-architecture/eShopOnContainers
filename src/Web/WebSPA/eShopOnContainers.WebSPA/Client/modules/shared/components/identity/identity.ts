import { Component, OnInit, OnChanges, Output, Input, EventEmitter } from '@angular/core';

import { IIdentity } from '../../models/identity.model';
import { SecurityService } from  '../../services/security.service';

@Component({
    selector: 'esh-identity',
    templateUrl: './identity.html',
    styleUrls: ['./identity.scss']
})
export class Identity implements OnInit  {
    constructor(private service: SecurityService) {
    }

    @Output()
    changed: EventEmitter<number> = new EventEmitter<number>();

    @Input()
    model: IIdentity;

    ngOnInit() {
        console.log("ngOnInit _securityService.AuthorizedCallback");

        if (window.location.hash) {
            this.service.AuthorizedCallback();
            console.log('isAutorized?');
            console.log(this.service.IsAuthorized);
        }
    }

    login() {
        this.service.Authorize();
    }

    logout() {
        this.service.Logoff();
    }
}
