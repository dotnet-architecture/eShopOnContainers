import { Injectable } from '@angular/core';
import { SecurityService } from './security.service';
import { ConfigurationService } from './configuration.service';
import { HubConnection, HubConnectionBuilder, LogLevel, HttpTransportType } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { Subject } from 'rxjs';

@Injectable()
export class SignalrService {
    private _hubConnection: HubConnection;
    private SignalrHubUrl: string = '';
    private msgSignalrSource = new Subject();
    msgReceived$ = this.msgSignalrSource.asObservable();

    constructor(
        private securityService: SecurityService,
        private configurationService: ConfigurationService, private toastr: ToastrService,
    ) {
        if (this.configurationService.isReady) {
            this.SignalrHubUrl = this.configurationService.serverSettings.signalrHubUrl;
            this.init();
        }
        else {
            this.configurationService.settingsLoaded$.subscribe(x => {
                this.SignalrHubUrl = this.configurationService.serverSettings.signalrHubUrl;
                this.init();
            });
        }
    }

    public stop() {
        this._hubConnection.stop();
    }

    private init() {
        if (this.securityService.IsAuthorized == true) {
            this.register();
            this.stablishConnection();
            this.registerHandlers();
        }
    }

    private register() {
        this._hubConnection = new HubConnectionBuilder()
            .withUrl(this.SignalrHubUrl + '/hub/notificationhub', {
                accessTokenFactory: () => this.securityService.GetToken()
            })
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .build();
    }

    private stablishConnection() {
        this._hubConnection.start()
            .then(() => {
                console.log('Hub connection started')
            })
            .catch(() => {
                console.log('Error while establishing connection')
            });
    }

    private registerHandlers() {
        this._hubConnection.on('UpdatedOrderState', (msg) => {
            console.log(`Order ${msg.orderId} updated to ${msg.status}`);
            this.toastr.success('Updated to status: ' + msg.status, 'Order Id: ' + msg.orderId);
            this.msgSignalrSource.next();
        });
    }
}