import { Injectable } from '@angular/core';
import { Response, Headers } from '@angular/http';

import { DataService } from '../shared/services/data.service';
import { SecurityService } from '../shared/services/security.service';
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
        buyerId: '',
        items: []
    };

    constructor(private service: DataService, private authService: SecurityService) {
        this.basket.items = [];

        // Init:
        if (this.authService.IsAuthorized) {
            if (this.authService.UserData) {
                this.basket.buyerId = this.authService.UserData.sub;
                this.getBasket().subscribe(basket => {
                    this.basket = basket;
                });
            }
        }
    }

    setBasket(item): Observable<boolean> {
        this.basket.items.push(item);
        return this.service.post(this.basketUrl + '/', this.basket).map((response: Response) => {
            return true;
        });
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