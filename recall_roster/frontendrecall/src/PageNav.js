import { useNavigate } from 'react-router-dom';

export const prevPage = () => {
    const history = useNavigate();

    const goBack = () => {
        history.goBack();
    };

    return {
        goBack
    };
};

export default prevPage;