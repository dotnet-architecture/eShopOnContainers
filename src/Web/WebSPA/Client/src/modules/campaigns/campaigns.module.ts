import { NgModule }             from '@angular/core';
import { BrowserModule  }       from '@angular/platform-browser';

import { SharedModule }         from '../shared/shared.module';
import { CampaignsComponent }      from './campaigns.component';
import { CampaignsDetailComponent }      from './campaigns-detail/campaigns-detail.component';
import { CampaignsService } from './campaigns.service';
import { Header }                from '../shared/components/header/header';

@NgModule({
    imports: [BrowserModule, SharedModule],
    declarations: [CampaignsComponent, CampaignsDetailComponent],
    providers: [CampaignsService]
})
export class CampaignsModule { }