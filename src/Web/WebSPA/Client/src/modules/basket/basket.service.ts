import { Injectable } from '@angular/core';
import { HttpResponse } from '@angular/common/http';
import { Router } from '@angular/router';

import { DataService } from '../shared/services/data.service';
import { SecurityService } from '../shared/services/security.service';
import { IBasket } from '../shared/models/basket.model';
import { IOrder } from '../shared/models/order.model';
import { IBasketCheckout } from '../shared/models/basketCheckout.model';
import { BasketWrapperService } from '../shared/services/basket.wrapper.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { StorageService } from '../shared/services/storage.service';

import { Observable, Observer, Subject } from 'rxjs';
import { map, catchError, tap } from 'rxjs/operators';

@Injectable()
export class BasketService {
    private basketUrl: string = '';
    private purchaseUrl: string = '';
    basket: IBasket = {
        buyerId: '',
        items: []
    };

    //observable that is fired when item is removed from basket
    private basketUpdateSource = new Subject();
    basketUpdate$ = this.basketUpdateSource.asObservable();

    constructor(private service: DataService, private authService: SecurityService, private basketWrapperService: BasketWrapperService, private router: Router, private configurationService: ConfigurationService, private storageService: StorageService) {
        this.basket.items = [];

        // Init:
        if (this.authService.IsAuthorized) {
            if (this.authService.UserData) {
                this.basket.buyerId = this.authService.UserData.sub;
                if (this.configurationService.isReady) {
                    this.basketUrl = this.configurationService.serverSettings.purchaseUrl;
                    this.purchaseUrl = this.configurationService.serverSettings.purchaseUrl;
                    this.loadData();
                }
                else {
                    this.configurationService.settingsLoaded$.subscribe(x => {
                        this.basketUrl = this.configurationService.serverSettings.purchaseUrl;
                        this.purchaseUrl = this.configurationService.serverSettings.purchaseUrl;
                        this.loadData();
                    });
                }
            }
        }

        this.basketWrapperService.orderCreated$.subscribe(x => {
            this.dropBasket();
        });
    }

    addItemToBasket(item): Observable<boolean> {
        let basketItem = this.basket.items.find(value => value.productId == item.productId);

        if (basketItem) {
            basketItem.quantity++;
        } else {
            this.basket.items.push(item);
        }

        return this.setBasket(this.basket);
    }

    setBasket(basket): Observable<boolean> {
        let url = this.purchaseUrl + '/b/api/v1/basket/';

        this.basket = basket;

        return this.service.post(url, basket).pipe<boolean>(tap((response: any) => true));
    }

    setBasketCheckout(basketCheckout): Observable<boolean> {
        let url = this.basketUrl + '/b/api/v1/basket/checkout';

        return this.service.postWithId(url, basketCheckout).pipe<boolean>(tap((response: any) => {
            this.basketWrapperService.orderCreated();
            return true;
        }));
    }

    getBasket(): Observable<IBasket> {
        let url = this.basketUrl + '/b/api/v1/basket/' + this.basket.buyerId;

        return this.service.get(url).pipe<IBasket>(tap((response: any) => {
            if (response.status === 204) {
                return null;
            }

            return response;
        }));
    }

    mapBasketInfoCheckout(order: IOrder): IBasketCheckout {
        let basketCheckout = <IBasketCheckout>{};

        basketCheckout.street = order.street
        basketCheckout.city = order.city;
        basketCheckout.country = order.country;
        basketCheckout.state = order.state;
        basketCheckout.zipcode = order.zipcode;
        basketCheckout.cardexpiration = order.cardexpiration;
        basketCheckout.cardnumber = order.cardnumber;
        basketCheckout.cardsecuritynumber = order.cardsecuritynumber;
        basketCheckout.cardtypeid = order.cardtypeid;
        basketCheckout.cardholdername = order.cardholdername;
        basketCheckout.total = 0;
        basketCheckout.expiration = order.expiration;

        return basketCheckout;
    }

    updateQuantity() {
        this.basketUpdateSource.next();
    }

    dropBasket() {
        this.basket.items = [];
        this.setBasket(this.basket).subscribe(res => {
            this.basketUpdateSource.next();
        });
    }

    private loadData() {
        this.getBasket().subscribe(basket => {
            if (basket != null)
                this.basket.items = basket.items;
        });
    }
}