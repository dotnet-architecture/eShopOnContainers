import { Component, OnInit }    from '@angular/core';
import { OrdersService }        from './orders.service';
import { IOrder }               from '../shared/models/order.model';
import { ConfigurationService } from '../shared/services/configuration.service';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { SignalrService } from '../shared/services/signalr.service';

@Component({
    selector: 'esh-orders .esh-orders .mb-5',
    styleUrls: ['./orders.component.scss'],
    templateUrl: './orders.component.html'
})
export class OrdersComponent implements OnInit {
    private oldOrders: IOrder[];
    private interval = null;
    errorReceived: boolean;

    orders: IOrder[];

    constructor(private service: OrdersService, private configurationService: ConfigurationService, private signalrService: SignalrService) { }

    ngOnInit() {
        if (this.configurationService.isReady) {
            this.getOrders();
        } else {
            this.configurationService.settingsLoaded$.subscribe(x => {
                this.getOrders();
            });
        }

        this.signalrService.msgReceived$
            .subscribe(x => this.getOrders());
    }

    getOrders() {
        this.errorReceived = false;
        this.service.getOrders()
            .pipe(catchError((err) => this.handleError(err)))
            .subscribe(orders => {
                this.orders = orders;
                this.oldOrders = this.orders;
                console.log('orders items retrieved: ' + orders.length);
        });
    }

    cancelOrder(orderNumber) {
        this.errorReceived = false;
        this.service.cancelOrder(orderNumber)
            .pipe(catchError((err) => this.handleError(err)))
            .subscribe(() => {
                console.log('order canceled: ' + orderNumber);
        });
    }

    private handleError(error: any) {
        this.errorReceived = true;
        return Observable.throw(error);
    }  
}

