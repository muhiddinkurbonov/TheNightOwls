import type { BarberWorkHoursDto, CreateBarberWorkHoursDto } from '@/types/api';
import { axiosInstance } from '../axios';

export const workHoursApi = {
  // Get all work hours (Admin only)
  getAll: async (): Promise<BarberWorkHoursDto[]> => {
    const { data } = await axiosInstance.get('/api/BarberWorkHours');
    return data;
  },

  // Get work hours by ID
  getById: async (workHourId: number): Promise<BarberWorkHoursDto> => {
    const { data } = await axiosInstance.get(`/api/BarberWorkHours/${workHourId}`);
    return data;
  },

  // Get work hours for a specific barber
  getByBarberId: async (barberId: number): Promise<BarberWorkHoursDto[]> => {
    const { data } = await axiosInstance.get(`/api/BarberWorkHours/barber/${barberId}`);
    return data;
  },

  // Get work hours for a barber on a specific day
  getByBarberIdAndDay: async (barberId: number, dayOfWeek: number): Promise<BarberWorkHoursDto[]> => {
    const { data } = await axiosInstance.get(`/api/BarberWorkHours/barber/${barberId}/day/${dayOfWeek}`);
    return data;
  },

  // Check if barber is available at a specific time
  isBarberAvailable: async (
    barberId: number,
    appointmentDateTime: string,
    durationMinutes: number = 30
  ): Promise<{ isAvailable: boolean }> => {
    const { data } = await axiosInstance.get(`/api/BarberWorkHours/barber/${barberId}/available`, {
      params: { appointmentDateTime, durationMinutes }
    });
    return data;
  },

  // Get available time slots for a barber on a specific date
  getAvailableTimeSlots: async (
    barberId: number,
    date: string,
    durationMinutes: number = 30
  ): Promise<string[]> => {
    const { data } = await axiosInstance.get(`/api/BarberWorkHours/barber/${barberId}/slots`, {
      params: { date, durationMinutes }
    });
    return data;
  },

  // Add new work hours (Admin only)
  create: async (workHours: CreateBarberWorkHoursDto): Promise<BarberWorkHoursDto> => {
    const { data } = await axiosInstance.post('/api/BarberWorkHours', workHours);
    return data;
  },

  // Update work hours (Admin only)
  update: async (workHourId: number, workHours: CreateBarberWorkHoursDto): Promise<BarberWorkHoursDto> => {
    const { data } = await axiosInstance.put(`/api/BarberWorkHours/${workHourId}`, workHours);
    return data;
  },

  // Delete work hours (Admin only)
  delete: async (workHourId: number): Promise<BarberWorkHoursDto> => {
    const { data } = await axiosInstance.delete(`/api/BarberWorkHours/${workHourId}`);
    return data;
  },
};
