import { Injectable } from '@angular/core';
import { SecurityService } from './security.service';
import { ConfigurationService } from './configuration.service';
import { HubConnection, HttpConnection, TransportType } from '@aspnet/signalr';
import { ToastsManager } from 'ng2-toastr/ng2-toastr';
import { Subject } from 'rxjs';

@Injectable()
export class SignalrService {

    private _hubConnection: HubConnection;
    private _httpConnection: HttpConnection;
    private SignalrHubUrl: string = '';
    private msgSignalrSource = new Subject();
    msgReceived$ = this.msgSignalrSource.asObservable();

    constructor(
        private securityService: SecurityService,
        private configurationService: ConfigurationService, private toastr: ToastsManager,
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
        this._httpConnection = new HttpConnection(this.SignalrHubUrl + '/hub/notificationhub', {
            transport: TransportType.LongPolling,
            accessTokenFactory: () => this.securityService.GetToken()
        });
        this._hubConnection = new HubConnection(this._httpConnection);
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
            this.toastr.success('Updated to status: ' + msg.status, 'Order Id: ' + msg.orderId);
            this.msgSignalrSource.next();
        });
    }

}