import { Component, OnInit }    from '@angular/core';
import { Router }               from '@angular/router';

import { BasketService }        from './basket.service';
import { IBasket }              from '../shared/models/basket.model';
import { IBasketItem }          from '../shared/models/basketItem.model';
import { BasketWrapperService } from '../shared/services/basket.wrapper.service';

@Component({
    selector: 'esh-basket .esh-basket',
    styleUrls: ['./basket.component.scss'],
    templateUrl: './basket.component.html'
})
export class BasketComponent implements OnInit {
    basket: IBasket;
    totalPrice: number = 0;

    constructor(private service: BasketService, private router: Router, private basketwrapper: BasketComponent) { }

    ngOnInit() {
        this.service.getBasket().subscribe(basket => {
            this.basket = basket;
            this.calculateTotalPrice();
        });
    }

    itemQuantityChanged(item: IBasketItem) {
        this.calculateTotalPrice();
        this.service.setBasket(this.basket);
    }

    checkOut() {
        console.log('checkout clicked!');
        this.basketwrapper.basket = this.basket;
        this.router.navigate(['order']);
    }

    private calculateTotalPrice() {
        this.basket.items.forEach(item => {
            this.totalPrice += (item.unitPrice * item.quantity)
        });
    }

    
}

