import { Title } from '@angular/platform-browser';
import { Component, ViewEncapsulation, OnInit, ViewContainerRef } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Subscription }   from 'rxjs/Subscription';

import { DataService } from './shared/services/data.service';
import { SecurityService } from './shared/services/security.service';
import { ConfigurationService } from './shared/services/configuration.service';
import { SignalrService } from './shared/services/signalr.service';
import { ToastsManager } from 'ng2-toastr';

/*
 * App Component
 * Top Level Component
 */

@Component({
    selector: 'esh-app',
    styleUrls: ['./app.component.scss'],
    templateUrl: './app.component.html'
})
export class AppComponent implements OnInit {
    Authenticated: boolean = false;
    subscription: Subscription;

    constructor(private titleService: Title,
        private securityService: SecurityService,
        private configurationService: ConfigurationService,
        private signalrService: SignalrService,
        private toastr: ToastsManager,
        vcr: ViewContainerRef
    ) {
        this.toastr.setRootViewContainerRef(vcr);
        this.Authenticated = this.securityService.IsAuthorized;
    }

    ngOnInit() {
        console.log('app on init');
        this.subscription = this.securityService.authenticationChallenge$.subscribe(res => this.Authenticated = res);

        //Get configuration from server environment variables:
        console.log('configuration');
        this.configurationService.load();        
    }

    public setTitle(newTitle: string) {
        this.titleService.setTitle('eShopOnContainers');        
    }
}
