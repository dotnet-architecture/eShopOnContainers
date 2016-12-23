import { Injectable } from '@angular/core';
import { Response } from '@angular/http';

import { DataService } from '../shared/services/data.service';
import { ICatalog } from '../shared/models/catalog.model';
import { ICatalogBrand } from '../shared/models/catalogBrand.model';
import { ICatalogType } from '../shared/models/catalogType.model';

import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';

@Injectable()
export class OrdersService {
    private catalogUrl: string = 'http://eshopcontainers:5101/api/v1/catalog/items';
    private brandUrl: string = 'http://eshopcontainers:5101/api/v1/catalog/catalogbrands';
    private typesUrl: string = 'http://eshopcontainers:5101/api/v1/catalog/catalogtypes';

    constructor(private service: DataService) {
    }

    getCatalog(pageIndex: number, pageSize: number, brand: number, type: number): Observable<ICatalog> {
        var url = this.catalogUrl;
        if (brand || type) {
            url = this.catalogUrl + '/type/' + ((type) ? type.toString() : 'null') + '/brand/' + ((brand) ? brand.toString() : 'null');
        }

        url = url + '?pageIndex=' + pageIndex + '&pageSize=' + pageSize;

        return this.service.get(url).map((response: Response) => {
            return response.json();
        });
    }

    getBrands(): Observable<ICatalogBrand[]> {
        return this.service.get(this.brandUrl).map((response: Response) => {
            return response.json();
        });
    }

    getTypes(): Observable<ICatalogType[]> {
        return this.service.get(this.typesUrl).map((response: Response) => {
            return response.json();
        });
    };
}