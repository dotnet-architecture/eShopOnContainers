import { Component, OnInit }    from '@angular/core';
import { OrdersService }        from './orders.service';
import { IOrder }               from '../shared/models/order.model';
import { ConfigurationService } from '../shared/services/configuration.service';
import { Observable } from 'rxjs/Observable';

@Component({
    selector: 'esh-orders',
    styleUrls: ['./orders.component.scss'],
    templateUrl: './orders.component.html'
})
export class OrdersComponent implements OnInit {
    private oldOrders: IOrder[];
    private interval = null;
    errorReceived: boolean;

    orders: IOrder[];

    constructor(private service: OrdersService, private configurationService: ConfigurationService) { }

    ngOnInit() {
        if (this.configurationService.isReady) {
            this.getOrders();
        } else {
            this.configurationService.settingsLoaded$.subscribe(x => {
                this.getOrders();
            });
        }

        // call orders until new order is retrieved
        this.interval = setTimeout(() => {
            this.service.getOrders().subscribe(orders => {
                this.orders = orders;
                if (this.orders.length !== this.oldOrders.length) {
                    clearInterval(this.interval);
                }
            });
        }, 1000);
    }

    getOrders() {
        this.errorReceived = false;
        this.service.getOrders()
            .catch((err) => this.handleError(err))
            .subscribe(orders => {
                this.orders = orders;
                this.oldOrders = this.orders;
                console.log('orders items retrieved: ' + orders.length);
        });
    }

    private handleError(error: any) {
        this.errorReceived = true;
        return Observable.throw(error);
    }  
}

