import { axiosInstance } from '../axios';
import type { CustomerDto, CreateCustomerDto, ServiceDto, BarberDto, AppointmentDto, AppointmentRequestDto } from '@/types/api';

export const customersApi = {
  // POST: api/customerappointment
  create: async (customer: CreateCustomerDto): Promise<CustomerDto> => {
    const { data } = await axiosInstance.post('/api/customerappointment', customer);
    return data;
  },

  // GET: /customer/{id}
  getById: async (id: number): Promise<CustomerDto> => {
    const { data } = await axiosInstance.get(`/customer/${id}`);
    return data;
  },

  // GET: api/customerappointment/services
  getServices: async (): Promise<ServiceDto[]> => {
    const { data } = await axiosInstance.get('/api/customerappointment/services');
    return data;
  },

  // GET: api/customerappointment/barbers-by-service/{serviceId}
  getBarbersByService: async (serviceId: number): Promise<BarberDto[]> => {
    const { data } = await axiosInstance.get(`/api/customerappointment/barbers-by-service/${serviceId}`);
    return data;
  },

  // POST: api/customerappointment/request-appointment
  requestAppointment: async (request: AppointmentRequestDto): Promise<AppointmentDto> => {
    const { data } = await axiosInstance.post('/api/customerappointment/request-appointment', request);
    return data;
  },
};
