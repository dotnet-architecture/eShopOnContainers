import { Component, OnInit } from '@angular/core';
import { CatalogService } from './catalog.service';
import { Catalog } from '../shared/models/catalog.model';
import { CatalogItem } from '../shared/models/catalogItem.model';

@Component({
    selector: 'appc-catalog',
    styleUrls: ['./catalog.component.scss'],
    templateUrl: './catalog.component.html'
})
export class CatalogComponent implements OnInit {
    private brands: any[];
    private types: any[];
    private items: CatalogItem[];
    private filteredItems: any[];
    private catalog: Catalog;

    constructor(private service: CatalogService) { }

    ngOnInit() {
        console.log('catalog component loaded');
        this.service.getCatalog().subscribe((catalog: Catalog) => {
            this.catalog = catalog;
            this.items = catalog.data;
            console.log(this.items.length + ' catalog items retrieved from api');
        });
    }
}
