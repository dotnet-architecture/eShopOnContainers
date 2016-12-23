import { Component, OnInit, OnChanges, Output, Input, EventEmitter } from '@angular/core';
import { Subscription } from 'rxjs/Subscription';

import { IIdentity } from '../../models/identity.model';
import { SecurityService } from  '../../services/security.service';

@Component({
    selector: 'esh-identity',
    templateUrl: './identity.html',
    styleUrls: ['./identity.scss']
})
export class Identity implements OnInit  {
    private authenticated: boolean = false;
    private subscription: Subscription;
    private userName: string = "";

    constructor(private service: SecurityService) {

    }

    ngOnInit() {
        this.subscription = this.service.authenticationChallenge$.subscribe(res =>
        {
            //console.log(res);
            //console.log(this.service.UserData);
            //console.log(this.service);
            this.authenticated = res;
            this.userName = this.service.UserData.email;
        });

        if (window.location.hash) {
            this.service.AuthorizedCallback();
        }
    }

    login() {
        this.service.Authorize();
    }

    logout() {
        this.service.Logoff();
    }
}
