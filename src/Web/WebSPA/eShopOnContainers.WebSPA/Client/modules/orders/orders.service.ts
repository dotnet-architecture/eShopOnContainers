import { Injectable } from '@angular/core';
import { Response } from '@angular/http';

import { DataService } from '../shared/services/data.service';
import { IOrder } from '../shared/models/order.model';

import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';

@Injectable()
export class OrdersService {
    private ordersUrl: string = 'http://eshopcontainers:5102/api/v1/orders';

    constructor(private service: DataService) {
    }

    getOrders(): Observable<any> {
        var url = this.ordersUrl;

        return this.service.get(url).map((response: Response) => {
            return response.json();
        });
    }
}
