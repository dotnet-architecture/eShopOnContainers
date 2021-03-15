import { Injectable } from '@angular/core';
import * as applicationinsightsWeb from '@microsoft/applicationinsights-web';
import { StorageService } from './storage.service';
import { Router, NavigationEnd, NavigationStart } from '@angular/router';
import { filter, isEmpty } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ApplicationInsightsService {
    private appInsights: applicationinsightsWeb.ApplicationInsights;

    constructor(private router: Router, private storageService: StorageService) {
       
    }
    load() {
        var aiKey = this.storageService.retrieve('instrumentationKey');
        this.appInsights = new applicationinsightsWeb.ApplicationInsights({
            config: {
                instrumentationKey: aiKey,
                enableAutoRouteTracking: true,
                autoTrackPageVisitTime: true
            }
        });

        this.appInsights.loadAppInsights();
        this.loadCustomTelemetryProperties();
        this.createRouterSubscription();
    }
    logTrace(message: string, properties?: { [key: string]: any }) {
        this.appInsights.trackTrace({ message: message }, properties);
    }
    logMetric(name: string, average: number, properties?: { [key: string]: any }) {
        this.appInsights.trackMetric({ name: name, average: average }, properties);
    }
    setUserId(userId: string) {
        this.appInsights.setAuthenticatedUserContext(userId);
    }
    clearUserId() {
        this.appInsights.clearAuthenticatedUserContext();
    }
    logPageView(name?: string, uri?: string, workstation?: string) {
        let MyPageView: applicationinsightsWeb.IPageViewTelemetry = { name: name, uri: uri, properties: { ['workstation']: workstation } }
        this.appInsights.trackPageView(MyPageView);
    }
    logException(error: Error) {
        let exception: applicationinsightsWeb.IExceptionTelemetry = {
            exception: error
        };
        this.appInsights.trackException(exception);
    }

    private loadCustomTelemetryProperties() {
        this.appInsights.addTelemetryInitializer(envelope => {
            var item = envelope.baseData;
            item.properties = item.properties || {};
            if (window.performance) { item.properties["Perf"] = window.performance; }
            item.properties["ApplicationPlatform"] = "webSPA";
            item.properties["ApplicationName"] = "eShop";
            if (item.url === 'socket.io') { return false; }
        }
        );
    }

    private createRouterSubscription() {
        this.router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe((event: NavigationEnd) => {
            //this.appInsights.stopTrackPage(event.url);
            //this.logPageView(null, event.url);
        });

        this.router.events.pipe(filter(event => event instanceof NavigationStart)).subscribe((event: NavigationStart) => {
            //this.appInsights.startTrackPage(event.url);
        });
    }
}