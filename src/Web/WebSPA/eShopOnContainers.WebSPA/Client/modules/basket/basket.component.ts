import { Component, OnInit }    from '@angular/core';

import { BasketService }        from './basket.service';
import { IBasket }              from '../shared/models/basket.model';
import { IBasketItem }          from '../shared/models/basketItem.model';

@Component({
    selector: 'esh-basket',
    styleUrls: ['./basket.component.scss'],
    templateUrl: './basket.component.html'
})
export class BasketComponent implements OnInit {
    basket: IBasket;
    totalPrice: number = 0;
    
    constructor(private service: BasketService) { }

    ngOnInit() {
        this.basket = this.service.getBasket()
        this.service.basket.items.forEach(item => {
            this.totalPrice += (item.unitPrice * item.quantity)
        });
    }

    itemQuantityChanged(item: IBasketItem) {
        this.calculateTotalPrice();
    }

    private calculateTotalPrice() {
        this.basket = this.service.basket.items;
        this.service.basket.items.forEach(item => {
            this.totalPrice += (item.unitPrice * item.quantity)
        });
    }
}

