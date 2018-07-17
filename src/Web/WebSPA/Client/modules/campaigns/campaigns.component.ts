import { Component, OnInit }    from '@angular/core';
import { CampaignsService }        from './campaigns.service';
import { ICampaign }               from '../shared/models/campaign.model';
import { IPager }               from '../shared/models/pager.model';
import { ConfigurationService } from '../shared/services/configuration.service';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'esh-campaigns',
    styleUrls: ['./campaigns.component.scss'],
    templateUrl: './campaigns.component.html'
})
export class CampaignsComponent implements OnInit {
    private interval = null;
    paginationInfo: IPager;
    campaigns: ICampaign;
    isCampaignDetailFunctionEnabled: boolean = false;
    errorReceived: boolean;

    constructor(private service: CampaignsService, private configurationService: ConfigurationService) { }

    ngOnInit() {
        if (this.configurationService.isReady) {
            this.getCampaigns(9, 0)
        } else {
            this.configurationService.settingsLoaded$.subscribe(x => {
                this.getCampaigns(9, 0);
            });
        }

        this.isCampaignDetailFunctionEnabled = this.configurationService.serverSettings.activateCampaignDetailFunction;
    }    

    onPageChanged(value: any) {
        console.log('campaigns pager event fired' + value);
        //event.preventDefault();
        this.paginationInfo.actualPage = value;
        this.getCampaigns(this.paginationInfo.itemsPage, value);
    }   

    getCampaigns(pageSize: number, pageIndex: number) {
        this.errorReceived = false;
        this.service.getCampaigns(pageIndex, pageSize)
            .catch((err) => this.handleError(err))
            .subscribe(campaigns => {
                this.campaigns = campaigns;
                this.paginationInfo = {
                    actualPage : campaigns.pageIndex,
                    itemsPage : campaigns.pageSize,
                    totalItems : campaigns.count,
                    totalPages: Math.ceil(campaigns.count / campaigns.pageSize),
                    items: campaigns.pageSize
                };
        });
    }

    onNavigateToDetails(uri: string) {
        window.open(uri, "_blank");
    }

    private handleError(error: any) {
        this.errorReceived = true;
        return Observable.throw(error);
    }  
}

