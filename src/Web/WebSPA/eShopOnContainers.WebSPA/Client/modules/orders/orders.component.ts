import { Component, OnInit }    from '@angular/core';
import { OrdersService }        from './orders.service';
import { IOrder }               from '../shared/models/order.model';

@Component({
    selector: 'esh-orders .orders',
    styleUrls: ['./orders.component.scss'],
    templateUrl: './orders.component.html'
})
export class OrdersComponent implements OnInit {
    orders: IOrder[];

    constructor(private service: OrdersService) { }

    ngOnInit() {
        this.getOrders();
    }


    getOrders() {
        this.service.getOrders().subscribe(orders => {
            this.orders = orders;
            console.log('orders items retrieved: ' + orders.length);
        });
    }
}

