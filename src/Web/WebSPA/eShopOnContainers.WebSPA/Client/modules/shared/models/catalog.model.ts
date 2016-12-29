import {ICatalogItem} from './catalogItem.model';

export interface ICatalog {
    pageIndex: number;
    data: ICatalogItem[];
    pageSize: number;
    count: number;
}
