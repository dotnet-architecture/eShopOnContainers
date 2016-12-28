import { Component, OnInit }                        from '@angular/core';
import { OrdersService }                            from '../orders.service';
import { IOrder }                                   from '../../shared/models/order.model';
import { SecurityService }                          from '../../shared/services/security.service';
import { FormGroup, FormBuilder, Validators  }      from '@angular/forms';
import { BasketWrapperService }                     from '../../shared/services/basket.wrapper.service';

@Component({
    selector: 'esh-orders-new .esh-orders-new',
    styleUrls: ['./orders-new.component.scss'],
    templateUrl: './orders-new.component.html'
})
export class OrdersNewComponent implements OnInit {
    private newOrderForm = {}; // new order form

    constructor(private service: OrdersService, private identityService: SecurityService, fb: FormBuilder, private basketWrapper: BasketWrapperService) {
        this.newOrderForm = fb.group({
            'address': [null, Validators.required],
            'city': [null, Validators.compose([Validators.required, Validators.minLength(5), Validators.maxLength(10)])],
            'state': [null, Validators.required],
            'country': [null, Validators.required],
            'cardnumber': [null, Validators.required],
            'cardholdername': [null, Validators.required],
            'expirationdate': [null, Validators.required],
            'securitycode': [null, Validators.required], 
        });
    }

    ngOnInit() {
        //Obtener el basket. 
        var basket = this.basketWrapper.basket;
        console.log('orders component');
        console.log(basket);
        console.log(this.identityService.UserData);
        //Obtener información del perfil de usuario.
        
    }

    submitForm(value: any) {
        console.log(value);
    }
}

