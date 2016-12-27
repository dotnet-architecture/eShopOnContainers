import { Component, OnInit }    from '@angular/core';
import { Subscription }         from 'rxjs/Subscription';

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
                this.service.setBasket(item).subscribe(res => {
                    this.service.getBasket().subscribe(basket => {
                        this.badge = basket.items.length;
                    });
                });
            });
    }
}

