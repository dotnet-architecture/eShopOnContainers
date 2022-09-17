import { Component, OnInit } from '@angular/core';
import { CatalogService } from '../catalog.service';
import { ICatalogItem } from '../../shared/models/catalogItem.model';
import { ActivatedRoute } from '@angular/router';

@Component({
    selector: 'esh-catalog_item-detail .esh-catalog_item-detail .mb-5',
    styleUrls: ['./catalog-item-detail.component.scss'],
    templateUrl: './catalog-item-detail.component.html'
})
export class CatalogItemDetailComponent implements OnInit {
    public order: ICatalogItem = <ICatalogItem>{};

    constructor(private service: CatalogService, private route: ActivatedRoute) { }

    ngOnInit() {
        this.route.params.subscribe(params => {
            let id = +params['id']; // (+) converts string 'id' to a number
           // this.getOrder(id);
        });
    }

   /* getOrder(id: number) {
        this.service.getCatalog().subscribe(order => {
            this.order = order;
            console.log('order retrieved: ' + order.ordernumber);
            console.log(this.order);
        });
    } */
}