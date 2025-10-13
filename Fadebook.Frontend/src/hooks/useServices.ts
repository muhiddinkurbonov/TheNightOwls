import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { axiosInstance } from '@/lib/axios';
import type { ServiceDto } from '@/types/api';

export interface CreateServiceDto {
  serviceId?: number;
  serviceName: string;
  servicePrice: number;
}

// Get all services
export const useServices = () => {
  return useQuery({
    queryKey: ['services'],
    queryFn: async () => {
      const { data } = await axiosInstance.get<ServiceDto[]>('/api/service');
      return data;
    },
    refetchInterval: 15000, // Auto-refetch every 15 seconds
    refetchIntervalInBackground: false, // Only refetch when tab is active
  });
};

// Get services for a specific barber
export const useBarberServices = (barberId: number) => {
  return useQuery({
    queryKey: ['barber-services', barberId],
    queryFn: async () => {
      const { data } = await axiosInstance.get<ServiceDto[]>(`/api/barber/${barberId}/services`);
      return data;
    },
    enabled: !!barberId && barberId > 0, // Only fetch if barberId is valid
  });
};

// Create service
export const useCreateService = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (service: CreateServiceDto) => {
      const { data } = await axiosInstance.post<ServiceDto>('/api/service', service);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['services'] });
    },
  });
};

// Update service
export const useUpdateService = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async ({ id, service }: { id: number; service: CreateServiceDto }) => {
      const { data } = await axiosInstance.put<ServiceDto>(`/api/service/${id}`, service);
      return data;
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['services'] });
      // Invalidate all barber-services queries since service details changed
      queryClient.invalidateQueries({ queryKey: ['barber-services'] });
    },
  });
};

// Delete service
export const useDeleteService = () => {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: async (id: number) => {
      await axiosInstance.delete(`/api/service/${id}`);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['services'] });
      // Invalidate all barber-services queries since a service was deleted
      queryClient.invalidateQueries({ queryKey: ['barber-services'] });
    },
  });
};
