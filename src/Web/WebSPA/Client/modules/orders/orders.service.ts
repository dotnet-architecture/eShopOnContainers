import { Injectable } from '@angular/core';
import { Response } from '@angular/http';

import { DataService } from '../shared/services/data.service';
import { IOrder } from '../shared/models/order.model';
import { IOrderItem } from '../shared/models/orderItem.model';
import { IOrderDetail } from "../shared/models/order-detail.model";
import { SecurityService } from '../shared/services/security.service';
import { ConfigurationService } from '../shared/services/configuration.service';
import { BasketWrapperService } from '../shared/services/basket.wrapper.service';

import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable()
export class OrdersService {
    private ordersUrl: string = '';

    constructor(private service: DataService, private basketService: BasketWrapperService, private identityService: SecurityService, private configurationService: ConfigurationService) {
        if (this.configurationService.isReady)
            this.ordersUrl = this.configurationService.serverSettings.purchaseUrl;
        else
            this.configurationService.settingsLoaded$.subscribe(x => this.ordersUrl = this.configurationService.serverSettings.purchaseUrl);

    }

    getOrders(): Observable<IOrder[]> {
        let url = this.ordersUrl + '/api/v1/o/orders';

        return this.service.get(url).pipe(map((response: Response) => {
            return response;
        }));
    }

    getOrder(id: number): Observable<IOrderDetail> {
        let url = this.ordersUrl + '/api/v1/o/orders/' + id;

        return this.service.get(url).pipe(map((response: Response) => {
            return response;
        }));
    }

    mapOrderAndIdentityInfoNewOrder(): IOrder {
        let order = <IOrder>{};
        let basket = this.basketService.basket;
        let identityInfo = this.identityService.UserData;

        console.log(basket);
        console.log(identityInfo);

        // Identity data mapping:
        order.street = identityInfo.address_street;
        order.city = identityInfo.address_city;
        order.country = identityInfo.address_country;
        order.state = identityInfo.address_state;
        order.zipcode = identityInfo.address_zip_code;
        order.cardexpiration = identityInfo.card_expiration;
        order.cardnumber = identityInfo.card_number;
        order.cardsecuritynumber = identityInfo.card_security_number;
        order.cardtypeid = identityInfo.card_type;
        order.cardholdername = identityInfo.card_holder;
        order.total = 0;
        order.expiration = identityInfo.card_expiration;

        // basket data mapping:
        order.orderItems = new Array<IOrderItem>();
        basket.items.forEach(x => {
            let item: IOrderItem = <IOrderItem>{};
            item.pictureurl = x.pictureUrl;
            item.productId =  +x.productId;
            item.productname = x.productName;
            item.unitprice = x.unitPrice;
            item.units = x.quantity;

            order.total += (item.unitprice * item.units);

            order.orderItems.push(item);
        });

        order.buyer = basket.buyerId;

        return order;
    }

}

