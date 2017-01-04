import { Component, OnInit }                        from '@angular/core';
import { OrdersService }                            from '../orders.service';
import { IOrder }                                   from '../../shared/models/order.model';
import { BasketWrapperService }                     from '../../shared/services/basket.wrapper.service';

import { FormGroup, FormBuilder, Validators  }      from '@angular/forms';
import { Router }                                   from '@angular/router';

@Component({
    selector: 'esh-orders_new',
    styleUrls: ['./orders-new.component.scss'],
    templateUrl: './orders-new.component.html'
})
export class OrdersNewComponent implements OnInit {
    private newOrderForm: FormGroup;  // new order form
    private order: IOrder;

    constructor(private service: OrdersService, fb: FormBuilder, private router: Router, private basketEvents: BasketWrapperService) {
        // Obtener información del perfil de usuario.
        this.order = service.mapBasketAndIdentityInfoNewOrder();
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
        this.order.cardholdername = this.newOrderForm.controls['cardholdername'].value;
        this.order.cardexpiration = new Date(20 + this.newOrderForm.controls['expirationdate'].value.split('/')[1], this.newOrderForm.controls['expirationdate'].value.split('/')[0]);
        this.order.cardsecuritynumber = this.newOrderForm.controls['securitycode'].value;

        this.service.postOrder(this.order).subscribe(res => {
            // this will emit an observable. Basket service is subscribed to this observable, and will react deleting the basket for the current user. 
            this.basketEvents.orderCreated();

            this.router.navigate(['orders']);
        });
    }
}

