import { Injectable } from '@angular/core';
import { Response } from '@angular/http';

import { DataService } from '../shared/services/data.service';
import { IBasket } from '../shared/models/basket.model';
import { IBasketItem } from '../shared/models/basketItem.model';

import 'rxjs/Rx';
import { Observable } from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { Observer } from 'rxjs/Observer';
import 'rxjs/add/operator/map';

@Injectable()
export class BasketService {
    private basketUrl: string = 'http://eshopcontainers:5103';
    basket: IBasket = {
        buyerId: 'fakeIdentity',
        items: []
    };

    constructor(private service: DataService) {
        this.basket.items = [];
    }

    setBasket(item) {
        this.basket.items.push(item);
        this.service.post(this.basket.buyerId, this.basket.items);
    }

    getBasket(): Observable<IBasket> {
        return this.service.get(this.basketUrl + '/' + this.basket.buyerId).map((response: Response) => {
            return response.json();
        });
    }

    dropBasket() {
        //TODO.. 
    }
}