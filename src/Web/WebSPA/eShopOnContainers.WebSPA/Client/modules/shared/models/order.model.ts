import {IOrderItem} from './orderItem.model';

export interface IOrder {
    ordernumber: number;
    date: Date;
    status: string;
    total: number;
    street?: string;
    city?: string;
    zipcode?: string;
    country?: string;
    orderitems?: IOrderItem[];
}
