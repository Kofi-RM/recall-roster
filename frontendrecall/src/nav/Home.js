import { ToolBar, Footer } from "../Miscelleneous";
import HomeBanner from "../components/HomeBanner";
const Home = () => (
  <div className="rr-page">
    <ToolBar />
    <main id="main-content"><HomeBanner /></main>
    <Footer />
  </div>
);
export default Home;
