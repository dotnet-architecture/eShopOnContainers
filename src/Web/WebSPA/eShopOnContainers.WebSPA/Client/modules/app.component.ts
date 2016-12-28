import { Title } from '@angular/platform-browser';
import { Component, ViewEncapsulation, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Subscription }   from 'rxjs/Subscription';

import { DataService } from './shared/services/data.service';
import { SecurityService } from './shared/services/security.service';

/*
 * App Component
 * Top Level Component
 */

@Component({
    selector: 'appc-app',
    styleUrls: ['./app.component.scss'],
    templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
    private Authenticated: boolean = false;
    subscription: Subscription;

    constructor(private titleService: Title, private securityService: SecurityService) {
        this.Authenticated = this.securityService.IsAuthorized;
    }

    ngOnInit() {
        console.log('app on init');
        this.subscription = this.securityService.authenticationChallenge$.subscribe(res => this.Authenticated = res);
    }

    public setTitle(newTitle: string) {
        this.titleService.setTitle('eShopOnContainers');
    }
}
