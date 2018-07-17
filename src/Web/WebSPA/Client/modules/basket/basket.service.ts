import { Injectable }               from '@angular/core';
import { Response, Headers }        from '@angular/http';
import { Router }                   from '@angular/router';

import { DataService }              from '../shared/services/data.service';
import { SecurityService }          from '../shared/services/security.service';
import { IBasket } from '../shared/models/basket.model';
import { IOrder } from '../shared/models/order.model';
import { IBasketCheckout } from '../shared/models/basketCheckout.model';
import { IBasketItem }              from '../shared/models/basketItem.model';
import { BasketWrapperService }     from '../shared/services/basket.wrapper.service';
import { ConfigurationService }     from '../shared/services/configuration.service';
import { StorageService }           from '../shared/services/storage.service';

import 'rxjs/Rx';
import { Observable }               from 'rxjs/Observable';
import 'rxjs/add/observable/throw';
import { Observer }                 from 'rxjs/Observer';
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/catch';
import { Subject }                  from 'rxjs/Subject';

@Injectable()
export class BasketService {
    private basketUrl: string = '';
    private purchaseUrl: string = '';
    basket: IBasket = {
        buyerId: '',
        items: []
    };

    //observable that is fired when the basket is dropped
    private basketDropedSource = new Subject();
    basketDroped$ = this.basketDropedSource.asObservable();
    
    constructor(private service: DataService, private authService: SecurityService, private basketEvents: BasketWrapperService, private router: Router, private configurationService: ConfigurationService, private storageService: StorageService) {
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

        this.basketEvents.orderCreated$.subscribe(x => {
            this.dropBasket();
        });
    }
    
    addItemToBasket(item): Observable<boolean> {
        this.basket.items.push(item);
        return this.setBasket(this.basket);
    }

    setBasket(basket): Observable<boolean> {
        let url = this.purchaseUrl + '/api/v1/basket/';
        this.basket = basket;
        return this.service.post(url, basket).map((response: Response) => {
            return true;
        });
    }

    setBasketCheckout(basketCheckout): Observable<boolean> {
        let url = this.basketUrl + '/api/v1/b/basket/checkout';
        return this.service.postWithId(url, basketCheckout).map((response: Response) => {
            this.basketEvents.orderCreated();
            return true;
        });
    }

    getBasket(): Observable<IBasket> {
        let url = this.basketUrl + '/api/v1/b/basket/' + this.basket.buyerId;
        return this.service.get(url).map((response: Response) => {
            if (response.status === 204) {
                return null;
            }

            return response.json();
        });
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

    dropBasket() {
        this.basket.items = [];        
        this.basketDropedSource.next();
    }

    private loadData() {
        this.getBasket().subscribe(basket => {
            if (basket != null)
                this.basket.items = basket.items;
        });
    }
}