import { Injectable } from '@angular/core';

import { DataService } from '../shared/services/data.service';

@Injectable()
export class CatalogService {
    private catalogUrl: string = 'http://eshopcontainers:5101/api/v1/catalog/items';

    constructor(private service: DataService) {
    }

    getCatalog(){
        return this.service.get(this.catalogUrl);
    }
}