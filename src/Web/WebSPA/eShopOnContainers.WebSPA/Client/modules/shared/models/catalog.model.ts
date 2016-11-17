import {CatalogItem} from './catalogItem.model';

export class Catalog {
    constructor(public pageIndex: number, public data: CatalogItem[], public pageSize: number, public count: number) {
    }
}
