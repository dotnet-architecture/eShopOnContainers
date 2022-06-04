import {ICampaignItem} from './campaignItem.model';

export interface ICampaign {
    data: ICampaignItem[];    
    pageIndex: number;
    pageSize: number;
    count: number;
}

