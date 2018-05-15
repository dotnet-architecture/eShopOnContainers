import { Injectable }       from '@angular/core';
import { HttpClient, HttpResponse, HttpHeaders } from '@angular/common/http';
import { IConfiguration }   from '../models/configuration.model';
import { StorageService }   from './storage.service';

import { Observable ,  Observer ,  Subject }       from 'rxjs';


@Injectable()
export class ConfigurationService {
    serverSettings: IConfiguration;
    // observable that is fired when settings are loaded from server
    private settingsLoadedSource = new Subject();
    settingsLoaded$ = this.settingsLoadedSource.asObservable();
    isReady: boolean = false;

    constructor(private http: HttpClient, private storageService: StorageService) { }
    
    load() {
        const baseURI = document.baseURI.endsWith('/') ? document.baseURI : `${document.baseURI}/`;
        let url = `${baseURI}Home/Configuration`;
        this.http.get(url).subscribe((data: IConfiguration) => {
            console.log('server settings loaded');
            this.serverSettings = data;
            console.log(this.serverSettings);
            this.storageService.store('identityUrl', this.serverSettings.identityUrl);
            this.storageService.store('marketingUrl', this.serverSettings.marketingUrl);
            this.storageService.store('purchaseUrl', this.serverSettings.purchaseUrl);
            this.storageService.store('signalrHubUrl', this.serverSettings.signalrHubUrl);
            this.storageService.store('activateCampaignDetailFunction', this.serverSettings.activateCampaignDetailFunction);
            this.isReady = true;
            this.settingsLoadedSource.next();
        });
    }
}
