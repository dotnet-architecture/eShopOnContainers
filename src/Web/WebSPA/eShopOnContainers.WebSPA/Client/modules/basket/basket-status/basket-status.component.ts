import { Component, OnInit }    from '@angular/core';
import { Subscription }         from 'rxjs/Subscription';

import { BasketService }        from '../basket.service';
import { BasketWrapperService } from '../../shared/services/basket.wrapper.service';
import { SecurityService }      from '../../shared/services/security.service';

@Component({
    selector: 'esh-basket-status',
    styleUrls: ['./basket-status.component.scss'],
    templateUrl: './basket-status.component.html'
})
export class BasketStatusComponent implements OnInit {
    basketItemAddedSubscription: Subscription;
    authSubscription: Subscription;
    basketDroppedSubscription: Subscription;

    badge: number = 0;

    constructor(private service: BasketService, private basketEvents: BasketWrapperService, private authService: SecurityService) { }

    ngOnInit() {
        // Subscribe to Add Basket Observable:
        this.basketItemAddedSubscription = this.basketEvents.addItemToBasket$.subscribe(
            item => {
                this.service.setBasket(item).subscribe(res => {
                    this.service.getBasket().subscribe(basket => {
                        this.badge = basket.items.length;
                    });
                });
            });

        // Subscribe to Drop Basket Observable: 
        this.basketDroppedSubscription = this.service.basketDroped$.subscribe(res => 
            this.service.getBasket().subscribe(basket => {
                this.badge = basket.items.length;
            })
        );

        // Subscribe to login and logout observable
        this.authSubscription = this.authService.authenticationChallenge$.subscribe(res => {
            this.service.getBasket().subscribe(basket => {
                this.badge = basket.items.length;
            });
        });

        // Init:
        this.service.getBasket().subscribe(basket => {
            this.badge = basket.items.length;
        });
    }
}

