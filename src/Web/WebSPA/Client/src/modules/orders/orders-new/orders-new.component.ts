import { Component, OnInit } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

import { OrdersService } from '../orders.service';
import { BasketService } from '../../basket/basket.service';
import { IOrder }                                   from '../../shared/models/order.model';
import { BasketWrapperService }                     from '../../shared/services/basket.wrapper.service';

import { UntypedFormGroup, UntypedFormBuilder, Validators  }      from '@angular/forms';
import { Router }                                   from '@angular/router';

@Component({
    selector: 'esh-orders_new .esh-orders_new .mb-5',
    styleUrls: ['./orders-new.component.scss'],
    templateUrl: './orders-new.component.html'
})
export class OrdersNewComponent implements OnInit {
    newOrderForm: UntypedFormGroup;  // new order form
    isOrderProcessing: boolean;
    errorReceived: boolean;
    order: IOrder;

    constructor(private orderService: OrdersService, private basketService: BasketService, fb: UntypedFormBuilder, private router: Router) {
        // Obtain user profile information
        this.order = orderService.mapOrderAndIdentityInfoNewOrder();
        this.newOrderForm = fb.group({
            'street': [this.order.street, Validators.required],
            'city': [this.order.city, Validators.required],
            'state': [this.order.state, Validators.required],
            'country': [this.order.country, Validators.required],
            'cardnumber': [this.order.cardnumber, Validators.required],
            'cardholdername': [this.order.cardholdername, Validators.required],
            'expirationdate': [this.order.expiration, Validators.required],
            'securitycode': [this.order.cardsecuritynumber, Validators.required],
        });
    }

    ngOnInit() {
    }

    submitForm(value: any) {
        this.order.street = this.newOrderForm.controls['street'].value;
        this.order.city = this.newOrderForm.controls['city'].value;
        this.order.state = this.newOrderForm.controls['state'].value;
        this.order.country = this.newOrderForm.controls['country'].value;
        this.order.cardnumber = this.newOrderForm.controls['cardnumber'].value;
        this.order.cardtypeid = 1;
        this.order.cardholdername = this.newOrderForm.controls['cardholdername'].value;
        this.order.cardexpiration = new Date(20 + this.newOrderForm.controls['expirationdate'].value.split('/')[1], this.newOrderForm.controls['expirationdate'].value.split('/')[0]);
        this.order.cardsecuritynumber = this.newOrderForm.controls['securitycode'].value;
        let basketCheckout = this.basketService.mapBasketInfoCheckout(this.order);
        this.basketService.setBasketCheckout(basketCheckout)
            .pipe(catchError((error) => {
                this.errorReceived = true;
                this.isOrderProcessing = false;
                return throwError(() => error); 
            }))
            .subscribe(res => {
                this.router.navigate(['orders']);
            });
        this.errorReceived = false;
        this.isOrderProcessing = true;
    }
}

