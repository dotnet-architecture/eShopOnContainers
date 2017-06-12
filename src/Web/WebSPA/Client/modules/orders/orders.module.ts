import { NgModule }             from '@angular/core';
import { BrowserModule  }       from '@angular/platform-browser';

import { SharedModule }         from '../shared/shared.module';
import { OrdersComponent }      from './orders.component';
import { OrdersDetailComponent }      from './orders-detail/orders-detail.component';
import { OrdersNewComponent }      from './orders-new/orders-new.component';
import { OrdersService } from './orders.service';
import { BasketService } from '../basket/basket.service';
import { Header }                from '../shared/components/header/header';

@NgModule({
    imports: [BrowserModule, SharedModule],
    declarations: [OrdersComponent, OrdersDetailComponent, OrdersNewComponent],
    providers: [OrdersService, BasketService]
})
export class OrdersModule { }
