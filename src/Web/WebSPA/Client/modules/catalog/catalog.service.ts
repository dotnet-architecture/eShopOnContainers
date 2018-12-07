import { Injectable } from '@angular/core';
import { Response } from '@angular/http';

import { DataService } from '../shared/services/data.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { ICatalog } from '../shared/models/catalog.model';
import { ICatalogBrand } from '../shared/models/catalogBrand.model';
import { ICatalogType } from '../shared/models/catalogType.model';

import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';

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

        if (type) {
            url = this.catalogUrl + '/type/' + type.toString() + '/brand/' + ((brand) ? brand.toString() : '');
        }
        else if (brand) {
            url = this.catalogUrl + '/type/all' + '/brand/' + ((brand) ? brand.toString() : '');
        }
      
        url = url + '?pageIndex=' + pageIndex + '&pageSize=' + pageSize;

        return this.service.get(url)
            .pipe(
                map((response: Response) => {
                    return response;
                })
            );
    }

    getBrands(): Observable<ICatalogBrand[]> {
        return this.service.get(this.brandUrl)
            .pipe(
                map((response: Response) => {
                    return response;
                })
            );
    }

    getTypes(): Observable<ICatalogType[]> {
        return this.service.get(this.typesUrl)
            .pipe(
                map((response: Response) => {
                    return response;
                })
            );
    };
}
