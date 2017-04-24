import { Component, OnInit }    from '@angular/core';
import { OrdersService }        from './orders.service';
import { IOrder }               from '../shared/models/order.model';
import { ConfigurationService } from '../shared/services/configuration.service';

@Component({
    selector: 'esh-orders',
    styleUrls: ['./orders.component.scss'],
    templateUrl: './orders.component.html'
})
export class OrdersComponent implements OnInit {
    orders: IOrder[];

    constructor(private service: OrdersService, private configurationService: ConfigurationService) { }

    ngOnInit() {
        if (this.configurationService.isReady) {
            this.getOrders()
        } else {
            this.configurationService.settingsLoaded$.subscribe(x => {
                this.getOrders();
            });
        }
    }


    getOrders() {
        this.service.getOrders().subscribe(orders => {
            this.orders = orders;
            console.log('orders items retrieved: ' + orders.length);
        });
    }
}

