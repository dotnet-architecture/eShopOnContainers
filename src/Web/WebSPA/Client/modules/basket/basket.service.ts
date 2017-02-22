import { Injectable }               from '@angular/core';
import { Response, Headers }        from '@angular/http';
import { Router }                   from '@angular/router';

import { DataService }              from '../shared/services/data.service';
import { SecurityService }          from '../shared/services/security.service';
import { IBasket }                  from '../shared/models/basket.model';
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
                    this.basketUrl = this.configurationService.serverSettings.basketUrl;
                    this.loadData();
                }
                else {
                    this.configurationService.settingsLoaded$.subscribe(x => {
                        this.basketUrl = this.configurationService.serverSettings.basketUrl;
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
        this.basket = basket;
        return this.service.post(this.basketUrl + '/', basket).map((response: Response) => {
            return true;
        });
    }

    getBasket(): Observable<IBasket> {
        return this.service.get(this.basketUrl + '/' + this.basket.buyerId).map((response: Response) => {
            if (response.status === 204) {
                return null;
            }

            return response.json();
        });
    }

    dropBasket() {
        this.basket.items = [];
        this.service.delete(this.basketUrl + '/' + this.basket.buyerId);
        this.basketDropedSource.next();
    }

    private loadData() {
        this.getBasket().subscribe(basket => {
            if (basket != null)
                this.basket.items = basket.items;
        });
    }
}