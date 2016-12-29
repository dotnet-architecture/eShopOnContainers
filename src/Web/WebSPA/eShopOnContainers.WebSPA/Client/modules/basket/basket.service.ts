import { Injectable }               from '@angular/core';
import { Response, Headers }        from '@angular/http';
import { Router }                   from '@angular/router';

import { DataService }              from '../shared/services/data.service';
import { SecurityService }          from '../shared/services/security.service';
import { IBasket }                  from '../shared/models/basket.model';
import { IBasketItem }              from '../shared/models/basketItem.model';
import { BasketWrapperService }     from '../shared/services/basket.wrapper.service';

import 'rxjs/Rx';
import { Observable }               from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { Observer }                 from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';

@Injectable()
export class BasketService {
    private basketUrl: string = 'http://eshopcontainers:5103';
    basket: IBasket = {
        buyerId: '',
        items: []
    };

    constructor(private service: DataService, private authService: SecurityService, private basketEvents: BasketWrapperService, private router: Router) {
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

        this.basketEvents.orderCreated$.subscribe(x => {
            this.dropBasket();
        });
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
        console.log('drop basket!');
        this.service.delete(this.basketUrl + '/' + this.basket.buyerId);
    }
}