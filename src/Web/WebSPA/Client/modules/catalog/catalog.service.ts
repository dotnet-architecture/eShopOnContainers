import { Injectable } from '@angular/core';
import { HttpResponse } from '@angular/common/http';

import { DataService } from '../shared/services/data.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { ICatalog } from '../shared/models/catalog.model';
import { ICatalogBrand } from '../shared/models/catalogBrand.model';
import { ICatalogType } from '../shared/models/catalogType.model';

import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';

@Injectable()
export class CatalogService {
    private catalogUrl: string = '';
    private brandUrl: string = '';
    private typesUrl: string = '';

    constructor(private service: DataService, private configurationService: ConfigurationService) {
        this.configurationService.settingsLoaded$.subscribe(x => {
            this.catalogUrl = this.configurationService.serverSettings.purchaseUrl + '/api/v1/c/catalog/items';
            this.brandUrl = this.configurationService.serverSettings.purchaseUrl + '/api/v1/c/catalog/catalogbrands';
            this.typesUrl = this.configurationService.serverSettings.purchaseUrl + '/api/v1/c/catalog/catalogtypes';
        });
    }

    getCatalog(pageIndex: number, pageSize: number, brand: number, type: number): Observable<ICatalog> {
        let url = this.catalogUrl;
        if (brand || type) {
            url = this.catalogUrl + '/type/' + ((type) ? type.toString() : 'null') + '/brand/' + ((brand) ? brand.toString() : 'null');
        }

        url = url + '?pageIndex=' + pageIndex + '&pageSize=' + pageSize;

        return this.service.get<ICatalog>(url).map((response: HttpResponse<ICatalog>) => {
            return response.body;
        });
    }

    getBrands(): Observable<ICatalogBrand[]> {
        return this.service.get<ICatalogBrand[]>(this.brandUrl).map((response: HttpResponse<ICatalogBrand[]>) => {
            return response.body;
        });
    }

    getTypes(): Observable<ICatalogType[]> {
        return this.service.get<ICatalogType[]>(this.typesUrl).map((response: HttpResponse<ICatalogType[]>) => {
            return response.body;
        });
    };
}
