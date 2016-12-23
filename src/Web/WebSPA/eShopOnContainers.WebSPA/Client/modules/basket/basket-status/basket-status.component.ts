import { Component, OnInit }    from '@angular/core';

import { Subscription }   from 'rxjs/Subscription';

import { BasketService }        from '../basket.service';
import { BasketWrapperService } from '../../shared/services/basket.wrapper.service';

@Component({
    selector: 'esh-basket-status',
    styleUrls: ['./basket-status.component.scss'],
    templateUrl: './basket-status.component.html'
})
export class BasketStatusComponent implements OnInit {
    subscription: Subscription;
    badge: number = 0;

    constructor(private service: BasketService, private basketEvents: BasketWrapperService) { }

    ngOnInit() {
        this.subscription = this.basketEvents.addItemToBasket$.subscribe(
            item => {
                console.log('element received in basket');
                console.log(item);
                this.service.setBasket(item).subscribe(res => {
                    console.log(res);
                    this.service.getBasket().subscribe(basket => {
                        this.badge = basket.items.length;
                        console.log('response from basket api');
                        console.log(basket.items.length);
                    });
                });
            });
    }

}

