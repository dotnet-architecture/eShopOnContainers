import { Component, OnInit }    from '@angular/core';
import { CatalogService }       from './catalog.service';
import { ICatalog }             from '../shared/models/catalog.model';
import { ICatalogItem }         from '../shared/models/catalogItem.model';
import { ICatalogType }         from '../shared/models/catalogType.model';
import { ICatalogBrand }        from '../shared/models/catalogBrand.model';
import { IPager }               from '../shared/models/pager.model';
import { BasketWrapperService}  from '../shared/services/basket.wrapper.service';

@Component({
    selector: 'esh-catalog .esh-catalog',
    styleUrls: ['./catalog.component.scss'],
    templateUrl: './catalog.component.html'
})
export class CatalogComponent implements OnInit {
    brands: ICatalogBrand[];
    types: ICatalogType[];
    catalog: ICatalog;
    brandSelected: number;
    typeSelected: number;
    paginationInfo: IPager;

    constructor(private service: CatalogService, private basketService: BasketWrapperService) { }

    ngOnInit() {
        this.getBrands();
        this.getCatalog(10, 0);
        this.getTypes();
    }

    onFilterApplied(event: any) {
        event.preventDefault();
        this.getCatalog(this.paginationInfo.itemsPage, this.paginationInfo.actualPage, this.brandSelected, this.typeSelected);
    }

    onBrandFilterChanged(event: any, value: number) {
        event.preventDefault();
        this.brandSelected = value;
    }

    onTypeFilterChanged(event: any, value: number) {
        event.preventDefault();
        this.typeSelected = value;
    }

    onPageChanged(value: any) {
        console.log('catalog pager event fired' + value);
        event.preventDefault();
        this.paginationInfo.actualPage = value;
        this.getCatalog(this.paginationInfo.itemsPage, value);
    }

    addToCart(item: ICatalogItem) {
        this.basketService.addItemToBasket(item);
    }

    getCatalog(pageSize:number, pageIndex: number, brand?: number, type?: number) {
        this.service.getCatalog(pageIndex, pageSize, brand, type).subscribe(catalog => {
            this.catalog = catalog;
            //console.log('catalog items retrieved: ' + catalog.count);

            this.paginationInfo = {
                actualPage : catalog.pageIndex,
                itemsPage : catalog.pageSize,
                totalItems : catalog.count,
                totalPages: Math.ceil(catalog.count / catalog.pageSize),
                items: catalog.pageSize
            };

            //console.log(this.paginationInfo);
        });
    }

    getTypes() {
        this.service.getTypes().subscribe(types => {
            this.types = types;
            let alltypes = { id: null, type: 'All' };
            this.types.unshift(alltypes);
            //console.log('types retrieved: ' + types.length);
        });
    }

    getBrands() {
        this.service.getBrands().subscribe(brands => {
            this.brands = brands;
            let allBrands = { id: null, brand: 'All' };
            this.brands.unshift(allBrands);
            //console.log('brands retrieved: ' + brands.length);
        });
    }
}

